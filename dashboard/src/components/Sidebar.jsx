import { NavLink } from "react-router-dom";
import { LayoutDashboard, Warehouse, Lightbulb, PiggyBank, Database } from "lucide-react";

export default function Sidebar() {
  const navItems = [
    { name: "Dashboard (Overblik)", path: "/", icon: LayoutDashboard },
    { name: "Stalde", path: "/stalde", icon: Warehouse },
    { name: "Lamper", path: "/lamper", icon: Lightbulb },
    { name: "Grise", path: "/grise", icon: PiggyBank },
    { name: "Rå Data (Log)", path: "/log", icon: Database },
  ];

  return (
    <div className="w-[260px] h-screen bg-white border-r border-gray-200 fixed left-0 top-0 flex flex-col shadow-sm z-10">
      <div className="p-6 border-b border-gray-100 flex items-center justify-center">
        <h1 className="text-2xl font-bold text-blue-600 tracking-tight">HG<span className="text-gray-800"> IoT</span></h1>
      </div>
      
      <nav className="flex-1 py-6 px-4 space-y-1 overflow-y-auto">
        <div className="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-4 px-2">
          Menu
        </div>
        {navItems.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            className={({ isActive }) =>
              `flex items-center gap-3 px-3 py-2.5 rounded-lg transition-colors duration-200 ${
                isActive
                  ? "bg-blue-50 text-blue-700 font-medium"
                  : "text-gray-600 hover:bg-gray-50 hover:text-gray-900"
              }`
            }
          >
            <item.icon className="w-5 h-5" />
            <span>{item.name}</span>
          </NavLink>
        ))}
      </nav>


    </div>
  );
}
