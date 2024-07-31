import React, { useEffect, useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import axios from 'axios';

delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
    iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
    iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
});

function MapView() {
    const [restaurants, setRestaurants] = useState([]);

    useEffect(() => {
        const token = localStorage.getItem('token');
        axios.get('https://localhost:7262/Auth/getFoodPlaces', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => {
                setRestaurants(response.data);
            })
            .catch(error => {
                console.error('A apărut o eroare la obținerea restaurantelor favorite.', error);
            });
    }, []);

    return (
        <MapContainer center={[45.9432, 24.9668]} zoom={6} style={{ height: "600px", width: "100%" }}>
            <TileLayer
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            />
            {restaurants.map((restaurant) => (
                <Marker key={restaurant.id} position={[restaurant.latitude, restaurant.longitude]}>
                    <Popup>
                        {restaurant.name} <br /> {restaurant.address}
                    </Popup>
                </Marker>
            ))}
        </MapContainer>
    );
}

export default MapView;
