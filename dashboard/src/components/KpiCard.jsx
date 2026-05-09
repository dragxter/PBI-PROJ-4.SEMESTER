export default function KpiCard({ title, value, icon: Icon, colorClass }) {
  return (
    <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100 flex items-center gap-4 transition-transform hover:-translate-y-1 duration-300">
      <div className={`w-12 h-12 rounded-full flex items-center justify-center ${colorClass}`}>
        {Icon && <Icon className="w-6 h-6" />}
      </div>
      <div>
        <h3 className="text-sm font-medium text-gray-500">{title}</h3>
        <p className="text-2xl font-bold text-gray-900 mt-1">{value}</p>
      </div>
    </div>
  );
}
