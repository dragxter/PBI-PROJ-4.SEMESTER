import { useState, useEffect } from "react";
import { lampsData } from "../data/mockData";
import { Lightbulb } from "lucide-react";
import KpiCard from "../components/KpiCard";

export default function Lamper() {
  const [lamps, setLamps] = useState([]);
  const [onlineCount, setOnlineCount] = useState(0);

  useEffect(() => {
    // API Fetch mock
    setLamps(lampsData);
    const active = lampsData.filter(l => l.status === "Aktiv").length;
    setOnlineCount(active);
  }, []);

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Lamper</h1>
        <p className="text-gray-500 mt-1">Overvåg status og lokation for alle RFID-lamper.</p>
      </div>

      {/* Top Sektion: Centreret KPI */}
      <div className="flex justify-center">
        <div className="w-full max-w-sm">
          <KpiCard 
            title="Online Lamper" 
            value={`${onlineCount} / ${lamps.length}`} 
            icon={Lightbulb} 
            colorClass="bg-yellow-100 text-yellow-600" 
          />
        </div>
      </div>

      {/* Bund Sektion: Tabel */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="p-6 border-b border-gray-100 bg-gray-50/50">
          <h2 className="text-lg font-semibold text-gray-900">Alle Lamper</h2>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-white border-b border-gray-100">
                <th className="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Lampe ID</th>
                <th className="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Status</th>
                <th className="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Stald</th>
                <th className="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Antal Grise</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {lamps.map((lamp) => (
                <tr key={lamp.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    <div className="flex items-center gap-3">
                      <Lightbulb className={`w-4 h-4 ${lamp.status === 'Aktiv' ? 'text-yellow-500' : 'text-gray-400'}`} />
                      {lamp.id}
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className={`px-2.5 py-1.5 text-xs font-medium rounded-full ${
                      lamp.status === 'Aktiv' ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
                    }`}>
                      {lamp.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">{lamp.stald}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600 font-medium">{lamp.pigCount}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
