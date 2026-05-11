import { useState, useEffect } from "react";
import { staldData, lampsData, pigsData } from "../data/mockData";
import { Warehouse, Lightbulb, PiggyBank } from "lucide-react";

export default function Stalde() {
  const [stalde, setStalde] = useState([]);
  const [selectedStald, setSelectedStald] = useState(null);
  const [lamps, setLamps] = useState([]);
  const [pigs, setPigs] = useState([]);

  useEffect(() => {
    // API Fetch mock for stalde
    setStalde(staldData);
    if (staldData.length > 0) {
      setSelectedStald(staldData[0]);
    }
  }, []);

  useEffect(() => {
    if (selectedStald) {
      // API Fetch mock for details of selected stald
      const filteredLamps = lampsData.filter(l => l.stald === selectedStald.name);
      const filteredPigs = pigsData.filter(p => p.stald === selectedStald.name);
      setLamps(filteredLamps);
      setPigs(filteredPigs);
    }
  }, [selectedStald]);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Stalde</h1>
        <p className="text-gray-500 mt-1">Håndter og få overblik over alle stalde.</p>
      </div>

      <div className="flex flex-col lg:flex-row gap-6">
        {/* Venstre Kolonne (Master) */}
        <div className="w-full lg:w-1/4 bg-white rounded-xl shadow-sm border border-gray-100 p-4 h-[calc(100vh-200px)] overflow-y-auto">
          <h2 className="text-sm font-semibold text-gray-400 uppercase tracking-wider mb-4 px-2">Vælg Stald</h2>
          <div className="space-y-2">
            {stalde.map(stald => (
              <button
                key={stald.id}
                onClick={() => setSelectedStald(stald)}
                className={`w-full text-left px-4 py-3 rounded-lg flex items-center gap-3 transition-colors ${
                  selectedStald?.id === stald.id
                    ? "bg-blue-50 text-blue-700 font-medium"
                    : "text-gray-700 hover:bg-gray-50 hover:text-gray-900"
                }`}
              >
                <Warehouse className={`w-5 h-5 ${selectedStald?.id === stald.id ? "text-blue-600" : "text-gray-400"}`} />
                {stald.name}
              </button>
            ))}
          </div>
        </div>

        {/* Højre Kolonne (Detail) */}
        <div className="w-full lg:w-3/4 space-y-6">
          {selectedStald ? (
            <>
              <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 flex items-center justify-between">
                <div>
                  <h2 className="text-xl font-bold text-gray-900">{selectedStald.name}</h2>
                  <p className="text-sm text-gray-500 mt-1">Detaljeret overblik over inventar og grise.</p>
                </div>
                <div className="flex gap-4">
                  <div className="flex items-center gap-2 px-3 py-1.5 bg-yellow-50 text-yellow-700 rounded-lg text-sm font-medium">
                    <Lightbulb className="w-4 h-4" /> {lamps.length} Lamper
                  </div>
                  <div className="flex items-center gap-2 px-3 py-1.5 bg-blue-50 text-blue-700 rounded-lg text-sm font-medium">
                    <PiggyBank className="w-4 h-4" /> {pigs.length} Grise
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {/* Tabel 1: Lamper */}
                <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
                  <div className="p-4 border-b border-gray-100 bg-gray-50/50">
                    <h3 className="font-semibold text-gray-900 flex items-center gap-2">
                      <Lightbulb className="w-4 h-4 text-gray-500" /> Lamper
                    </h3>
                  </div>
                  <div className="overflow-x-auto h-[400px]">
                    <table className="w-full text-left border-collapse">
                      <thead className="sticky top-0 bg-white">
                        <tr className="border-b border-gray-100">
                          <th className="px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Lampe ID</th>
                          <th className="px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Status</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-gray-100">
                        {lamps.map((lamp) => (
                          <tr key={lamp.id} className="hover:bg-gray-50">
                            <td className="px-4 py-3 text-sm font-medium text-gray-900">{lamp.id}</td>
                            <td className="px-4 py-3">
                              <span className={`px-2 py-1 text-xs font-medium rounded-full ${
                                lamp.status === 'Aktiv' ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'
                              }`}>
                                {lamp.status}
                              </span>
                            </td>
                          </tr>
                        ))}
                        {lamps.length === 0 && (
                          <tr>
                            <td colSpan="2" className="px-4 py-8 text-center text-sm text-gray-500">Ingen lamper fundet.</td>
                          </tr>
                        )}
                      </tbody>
                    </table>
                  </div>
                </div>

                {/* Tabel 2: Grise */}
                <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
                  <div className="p-4 border-b border-gray-100 bg-gray-50/50">
                    <h3 className="font-semibold text-gray-900 flex items-center gap-2">
                      <PiggyBank className="w-4 h-4 text-gray-500" /> Grise
                    </h3>
                  </div>
                  <div className="overflow-x-auto h-[400px]">
                    <table className="w-full text-left border-collapse">
                      <thead className="sticky top-0 bg-white">
                        <tr className="border-b border-gray-100">
                          <th className="px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Gris ID</th>
                          <th className="px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Status</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-gray-100">
                        {pigs.map((pig) => (
                          <tr key={pig.id} className="hover:bg-gray-50">
                            <td className="px-4 py-3 text-sm font-medium text-gray-900">{pig.id}</td>
                            <td className="px-4 py-3">
                              <span className={`px-2 py-1 text-xs font-medium rounded-full ${
                                pig.status === 'Aktiv' ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'
                              }`}>
                                {pig.status}
                              </span>
                            </td>
                          </tr>
                        ))}
                        {pigs.length === 0 && (
                          <tr>
                            <td colSpan="2" className="px-4 py-8 text-center text-sm text-gray-500">Ingen grise fundet.</td>
                          </tr>
                        )}
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            </>
          ) : (
            <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-8 flex items-center justify-center h-full">
              <p className="text-gray-500">Vælg en stald fra menuen til venstre.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
