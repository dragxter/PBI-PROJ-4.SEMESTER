import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";
import Dashboard from "./pages/Dashboard";
import Stalde from "./pages/Stalde";
import Lamper from "./pages/Lamper";
import Grise from "./pages/Grise";


function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<Dashboard />} />
          <Route path="stalde" element={<Stalde />} />
          <Route path="lamper" element={<Lamper />} />
          <Route path="grise" element={<Grise />} />

        </Route>
      </Routes>
    </Router>
  );
}

export default App;
