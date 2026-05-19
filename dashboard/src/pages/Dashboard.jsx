import { useState, useEffect } from "react";
import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts";
import { PiggyBank, Lightbulb, Activity } from "lucide-react";
import KpiCard from "../components/KpiCard";
import { fetchKpi, fetchActivity, fetchLiveStatus } from "../api";

export default function Dashboard() {
  const [kpis, setKpis] = useState({ activePigs: 0, onlineLamps: 0, systemUptime: "0%" });
  const [chartData, setChartData] = useState([]);
  const [liveData, setLiveData] = useState([]);

  useEffect(() => {
    const loadData = async () => {
      try {
        const kpiData = await fetchKpi();
        setKpis(kpiData);

        const activityData = await fetchActivity();
        setChartData(activityData);

        const statusData = await fetchLiveStatus();
        setLiveData(statusData);
      } catch (error) {
        console.error("Failed to load dashboard data:", error);
      }
    };
    loadData();
  }, []);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-500 mt-1">Overblik over RFID aktivitet og system status.</p>
      </div>

      {/* KPI Sektion */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <KpiCard 
          title="Aktive Grise" 
          value={kpis.activePigs} 
          icon={PiggyBank} 
          colorClass="bg-blue-100 text-blue-600" 
        />
        <KpiCard 
          title="Online Lamper" 
          value={kpis.onlineLamps} 
          icon={Lightbulb} 
          colorClass="bg-yellow-100 text-yellow-600" 
        />
        <KpiCard 
          title="System Oppetid" 
          value={kpis.systemUptime} 
          icon={Activity} 
          colorClass="bg-green-100 text-green-600" 
        />
      </div>

      {/* Graf Sektion */}
      <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-100">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Aktivitet over tid</h2>
        <div className="h-[300px] w-full">
          <ResponsiveContainer width="100%" height="100%">
            <AreaChart data={chartData} margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
              <defs>
                <linearGradient id="colorActivity" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="#3b82f6" stopOpacity={0.3}/>
                  <stop offset="95%" stopColor="#3b82f6" stopOpacity={0}/>
                </linearGradient>
              </defs>
              <XAxis dataKey="time" stroke="#9ca3af" fontSize={12} tickLine={false} axisLine={false} />
              <YAxis stroke="#9ca3af" fontSize={12} tickLine={false} axisLine={false} />
              <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f3f4f6" />
              <Tooltip 
                contentStyle={{ borderRadius: '8px', border: 'none', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}
              />
              <Area 
                type="monotone" 
                dataKey="activity" 
                stroke="#3b82f6" 
                strokeWidth={2}
                fillOpacity={1} 
                fill="url(#colorActivity)" 
              />
            </AreaChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Tabel Sektion */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="p-6 border-b border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900">Live Status (Seneste læsninger)</h2>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-gray-50 border-b border-gray-100">
                <th className="px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Gris ID</th>
                <th className="px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Lampe ID</th>
                <th className="px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Tidspunkt</th>
                <th className="px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {liveData.map((row) => (
                <tr key={row.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{row.pigId}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{row.lampId}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{row.timestamp}</td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className={`px-2.5 py-1 text-xs font-medium rounded-full ${
                      row.status === 'Aktiv' ? 'bg-green-100 text-green-700' : 
                      row.status === 'Spiser' ? 'bg-blue-100 text-blue-700' : 
                      'bg-gray-100 text-gray-700'
                    }`}>
                      {row.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
