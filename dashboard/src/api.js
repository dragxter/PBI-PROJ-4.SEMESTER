const API_BASE_URL = 'http://localhost:5167/api';

export const fetchKpi = async () => {
    const response = await fetch(`${API_BASE_URL}/dashboard/kpi`);
    if (!response.ok) throw new Error('Network response was not ok');
    return response.json();
};

export const fetchActivity = async () => {
    const response = await fetch(`${API_BASE_URL}/dashboard/activity`);
    if (!response.ok) throw new Error('Network response was not ok');
    return response.json();
};

export const fetchLiveStatus = async () => {
    const response = await fetch(`${API_BASE_URL}/dashboard/live-status`);
    if (!response.ok) throw new Error('Network response was not ok');
    return response.json();
};

export const fetchPigs = async () => {
    const response = await fetch(`${API_BASE_URL}/pigs`);
    if (!response.ok) throw new Error('Network response was not ok');
    return response.json();
};

export const fetchLamps = async () => {
    const response = await fetch(`${API_BASE_URL}/lamps`);
    if (!response.ok) throw new Error('Network response was not ok');
    return response.json();
};

export const fetchStables = async () => {
    const response = await fetch(`${API_BASE_URL}/stables`);
    if (!response.ok) throw new Error('Network response was not ok');
    return response.json();
};

export const fetchPigHistory = async (pigId) => {
    const response = await fetch(`${API_BASE_URL}/pigs/${pigId}/history`);
    if (!response.ok) throw new Error('Network response was not ok');
    return response.json();
};
