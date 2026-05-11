export default function PlaceholderPage({ title }) {
  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-8 flex flex-col items-center justify-center min-h-[400px]">
      <h1 className="text-2xl font-bold text-gray-900 mb-2">{title}</h1>
      <p className="text-gray-500 text-center max-w-md">
        Denne side er under udvikling. Data for {title.toLowerCase()} vil blive vist her i fremtiden.
      </p>
    </div>
  );
}
