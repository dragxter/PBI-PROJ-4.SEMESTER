import React, { useState, useEffect } from "react";
import { fetchPigs, fetchPigHistory } from "../api";
import { ArrowUpDown, PiggyBank, ChevronRight, ChevronDown } from "lucide-react";

export default function Grise() {
  const [pigs, setPigs] = useState([]);
  const [sortConfig, setSortConfig] = useState({ key: null, direction: 'ascending' });
  const [expandedPigId, setExpandedPigId] = useState(null);
  const [historyCache, setHistoryCache] = useState({});
  const [loadingHistory, setLoadingHistory] = useState(false);

  useEffect(() => {
    const loadData = async () => {
      try {
        const data = await fetchPigs();
        setPigs(data);
      } catch (error) {
        console.error("Failed to fetch pigs:", error);
      }
    };
    loadData();
  }, []);

  const toggleRow = async (pigId) => {
    if (expandedPigId === pigId) {
      setExpandedPigId(null);
      return;
    }
    
    setExpandedPigId(pigId);
    
    if (!historyCache[pigId]) {
      setLoadingHistory(true);
      try {
        const history = await fetchPigHistory(pigId);
        setHistoryCache(prev => ({ ...prev, [pigId]: history }));
      } catch (error) {
        console.error("Failed to fetch history for pig:", error);
      } finally {
        setLoadingHistory(false);
      }
    }
  };

  const requestSort = (key) => {
    let direction = 'ascending';
    if (sortConfig.key === key && sortConfig.direction === 'ascending') {
      direction = 'descending';
    }
    setSortConfig({ key, direction });

    const sortedData = [...pigs].sort((a, b) => {
      if (a[key] < b[key]) {
        return direction === 'ascending' ? -1 : 1;
      }
      if (a[key] > b[key]) {
        return direction === 'ascending' ? 1 : -1;
      }
      return 0;
    });
    
    setPigs(sortedData);
  };

  const SortableHeader = ({ label, sortKey }) => (
    <th 
      className="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100 transition-colors group"
      onClick={() => requestSort(sortKey)}
    >
      <div className="flex items-center justify-between">
        {label}
        <ArrowUpDown className={`w-3.5 h-3.5 ${sortConfig.key === sortKey ? 'text-blue-500' : 'text-gray-300 group-hover:text-gray-400'}`} />
      </div>
    </th>
  );

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Grise (RFID)</h1>
        <p className="text-gray-500 mt-1">Komplet liste over alle sporede grise og deres lokation.</p>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="p-4 border-b border-gray-100 bg-gray-50/50 flex justify-between items-center">
          <h2 className="text-lg font-semibold text-gray-900 flex items-center gap-2">
            <PiggyBank className="w-5 h-5 text-gray-500" /> Database over grise
          </h2>
          <span className="text-sm text-gray-500 font-medium">{pigs.length} registreringer</span>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-white border-b border-gray-100">
                <SortableHeader label="Gris ID" sortKey="id" />
                <SortableHeader label="Status" sortKey="status" />
                <SortableHeader label="Stald" sortKey="stald" />
                <SortableHeader label="Lokation" sortKey="location" />
                <SortableHeader label="Sidst flyttet" sortKey="lastMoved" />
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {pigs.map((pig) => (
                <React.Fragment key={pig.id}>
                  <tr 
                    className={`hover:bg-gray-50 transition-colors cursor-pointer ${expandedPigId === pig.id ? 'bg-blue-50/30' : ''}`}
                    onClick={() => toggleRow(pig.id)}
                  >
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900 flex items-center gap-2">
                      {expandedPigId === pig.id ? <ChevronDown className="w-4 h-4 text-gray-400" /> : <ChevronRight className="w-4 h-4 text-gray-400" />}
                      {pig.id}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2.5 py-1.5 text-xs font-medium rounded-full ${
                        pig.status === 'Aktiv' ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'
                      }`}>
                        {pig.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">{pig.stald}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600 font-medium">{pig.location}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{pig.lastMoved}</td>
                  </tr>
                  {expandedPigId === pig.id && (
                    <tr className="bg-gray-50">
                      <td colSpan="5" className="px-10 py-6 border-b border-gray-200">
                        {loadingHistory ? (
                          <div className="text-sm text-gray-500 flex items-center gap-2">
                            <div className="w-4 h-4 border-2 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
                            Henter historik...
                          </div>
                        ) : (
                          <div className="border border-gray-200 rounded-lg overflow-hidden bg-white shadow-sm">
                            <table className="w-full text-left text-sm">
                              <thead className="bg-gray-100/50">
                                <tr>
                                  <th className="px-4 py-2.5 font-semibold text-gray-600">Tidspunkt</th>
                                  <th className="px-4 py-2.5 font-semibold text-gray-600">Fra Lokation</th>
                                  <th className="px-4 py-2.5 font-semibold text-gray-600">Til Lokation</th>
                                </tr>
                              </thead>
                              <tbody className="divide-y divide-gray-100">
                                {historyCache[pig.id]?.map((h, i) => (
                                  <tr key={i} className="hover:bg-gray-50">
                                    <td className="px-4 py-2.5 text-gray-500">{h.timestamp}</td>
                                    <td className="px-4 py-2.5 text-gray-900 font-medium">{h.from}</td>
                                    <td className="px-4 py-2.5 text-gray-900 font-medium">{h.to}</td>
                                  </tr>
                                ))}
                                {(!historyCache[pig.id] || historyCache[pig.id].length === 0) && (
                                  <tr>
                                    <td colSpan="3" className="px-4 py-6 text-center text-gray-500">Ingen bevægelseshistorik fundet for denne gris.</td>
                                  </tr>
                                )}
                              </tbody>
                            </table>
                          </div>
                        )}
                      </td>
                    </tr>
                  )}
                </React.Fragment>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
