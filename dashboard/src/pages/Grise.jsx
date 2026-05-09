import { useState, useEffect } from "react";
import { pigsData } from "../data/mockData";
import { ArrowUpDown, PiggyBank } from "lucide-react";

export default function Grise() {
  const [pigs, setPigs] = useState([]);
  const [sortConfig, setSortConfig] = useState({ key: null, direction: 'ascending' });

  useEffect(() => {
    // API Fetch mock
    setPigs(pigsData);
  }, []);

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
                <tr key={pig.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">{pig.id}</td>
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
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
