import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { MapContainer, TileLayer, Marker } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrashAlt, faStar, faPlus } from '@fortawesome/free-solid-svg-icons';
import './../css/FavoriteRestaurants.css';
import L from 'leaflet';

const customIcon = L.divIcon({
    className: 'custom-icon',
    html: `
        <svg width="32" height="32" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path fill="#ff0000" d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5S10.62 6.5 12 6.5 14.5 7.62 14.5 9 13.38 11.5 12 11.5z"/>
        </svg>
    `,
    iconSize: [32, 32],
    iconAnchor: [16, 32],
    popupAnchor: [0, -32],
});

const highlightedIcon = L.divIcon({
    className: 'custom-icon highlighted',
    html: `
        <svg width="32" height="32" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path fill="#00ff00" d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5S10.62 6.5 12 6.5 14.5 7.62 14.5 9 13.38 11.5 12 11.5z"/>
        </svg>
    `,
    iconSize: [32, 32],
    iconAnchor: [16, 32],
    popupAnchor: [0, -32],
});

const FavoriteRestaurants = () => {
    const [restaurants, setRestaurants] = useState([]);
    const [categories, setCategories] = useState([]);
    const [recommendedRestaurants, setRecommendedRestaurants] = useState([]);
    const [selectedRestaurant, setSelectedRestaurant] = useState(null);
    const [refresh, setRefresh] = useState(false);
    const [highlightedLocationId, setHighlightedLocationId] = useState(null);
    const [personId, setPersonId] = useState(null);
    const [myFavoriteRestaurants, setMyFavoriteRestaurants] = useState([]);

    useEffect(() => {
        const token = localStorage.getItem('token');
        const firstName = localStorage.getItem('firstName');
        const lastName = localStorage.getItem('lastName');

        axios.get(`https://localhost:7262/api/Person/${lastName}/${firstName}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => {
                setPersonId(response.data.id);
            })
            .catch(error => {
                console.error('A apărut o eroare la obținerea personId-ului utilizatorului.', error);
            });

        axios.get('https://localhost:7262/Auth/getFoodPlaces', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => {
                const foodPlaces = response.data;
                const fetchRatingsPromises = foodPlaces.map(restaurant =>
                    Promise.all([
                        fetchFoodQuality(restaurant.name),
                        fetchServiceKindness(restaurant.name)
                    ]).then(([foodQuality, serviceKindness]) => ({
                        ...restaurant,
                        foodQuality,
                        serviceKindness
                    }))
                );
                Promise.all(fetchRatingsPromises).then(setRestaurants);
                setRefresh(false);
            })
            .catch(error => {
                console.error('A apărut o eroare la obținerea restaurantelor favorite.', error);
            });

        axios.get('https://localhost:7262/api/FoodPlaceCategory', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => {
                setCategories(response.data);
            })
            .catch(error => {
                console.error('A apărut o eroare la obținerea categoriilor de restaurante.', error);
            });

        axios.get(`https://localhost:7262/api/Person/${firstName}/${lastName}/recommendations`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => {
                const recommendations = response.data;
                const fetchRatingsPromises = recommendations.map(restaurant =>
                    Promise.all([
                        fetchFoodQuality(restaurant.name),
                        fetchServiceKindness(restaurant.name)
                    ]).then(([foodQuality, serviceKindness]) => ({
                        ...restaurant,
                        foodQuality,
                        serviceKindness
                    }))
                );
                Promise.all(fetchRatingsPromises).then(setRecommendedRestaurants);
            })
            .catch(error => {
                console.error('A apărut o eroare la obținerea recomandărilor.', error);
            });
    }, [refresh]);

    const handleRestaurantHover = (restaurant) => {
        const category = categories.find(cat => cat.id === restaurant.foodPlaceCategoryId);
        setSelectedRestaurant({ ...restaurant, categoryName: category ? category.name : 'N/A' });
        fetchLocations(restaurant.name);
    };

    const fetchLocations = (foodPlaceName) => {
        axios.get(`https://localhost:7262/api/FoodPlace/${foodPlaceName}/locations`)
            .then(response => {
                setSelectedRestaurant(prev => ({
                    ...prev,
                    locations: response.data
                }));
            })
            .catch(error => {
                console.error('A apărut o eroare la obținerea locațiilor.', error);
            });
    };

    const handleDeleteRestaurant = (restaurantId) => {
        const token = localStorage.getItem('token');

        if (window.confirm('Ești sigur că vrei să ștergi acest restaurant?')) {
            axios.delete(`https://localhost:7262/api/FoodPlace/${restaurantId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            })
                .then(response => {
                    setRestaurants(restaurants.filter(restaurant => restaurant.id !== restaurantId));
                    setRefresh(true);
                })
                .catch(error => {
                    console.error('A apărut o eroare la ștergerea restaurantului.', error);
                });
        }
    };

    const fetchFoodQuality = (foodPlaceName) => {
        return axios.get(`https://localhost:7262/api/CustomerReview/foodQualityByName/${foodPlaceName}`)
            .then(response => response.data)
            .catch(error => {
                console.error('A apărut o eroare la obținerea calității mâncării.', error);
                return null;
            });
    };

    const fetchServiceKindness = (foodPlaceName) => {
        return axios.get(`https://localhost:7262/api/CustomerReview/serviceKindnessByName/${foodPlaceName}`)
            .then(response => response.data)
            .catch(error => {
                console.error('A apărut o eroare la obținerea amabilității serviciului.', error);
                return null;
            });
    };

    const handleAddFavoriteRestaurant = (restaurantName) => {
        const token = localStorage.getItem('token');

        axios.get(`https://localhost:7262/api/FoodPlace/${restaurantName}/name`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then((response) => {
                const restaurant = response.data;
                axios.post('https://localhost:7262/api/FoodPlace', {
                    name: restaurant.name,
                    foodPlaceCategoryId: restaurant.foodPlaceCategoryId,
                    personId: personId,
                    ImageUrl: restaurant.ImageUrl
                }, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                })
                    .then(() => {
                        alert('Restaurantul a fost adăugat la favoritele tale.');
                        axios.get(`https://localhost:7262/Auth/getFoodPlaces`, {
                            headers: {
                                'Authorization': `Bearer ${token}`
                            }
                        })
                            .then(response => {
                                setRestaurants(response.data);
                                setRecommendedRestaurants(prev =>
                                    prev.filter(r => r.name !== restaurant.name)
                                );
                            })
                            .catch(error => {
                                console.error('A apărut o eroare la reîmprospătarea restaurantelor favorite ale utilizatorului logat.', error);
                            });
                    })
                    .catch((error) => {
                        console.error('A apărut o eroare la adăugarea restaurantului.', error);
                    });
            })
            .catch((error) => {
                console.error(`A apărut o eroare la obținerea detaliilor restaurantului: ${error}`);
            });
    };

    return (
        <div className="favorite-restaurants-container">
            <div className="favorite-restaurants">
                <h1>Restaurante favorite</h1>
                {restaurants.length > 0 ? (
                    <ul>
                        {restaurants.map((restaurant) => (
                            <li key={restaurant.id} onMouseEnter={() => handleRestaurantHover(restaurant)} onMouseLeave={() => setSelectedRestaurant(null)}>
                                <div className="restaurant-info">
                                    <div className={getRestaurantLogoClass(restaurant.name)}></div>
                                    <h2>{restaurant.name}</h2>
                                    <div className="rating">
                                        <span>Food Quality: {restaurant.foodQuality ? restaurant.foodQuality.toFixed(1) : 'N/A'}</span>
                                        <FontAwesomeIcon icon={faStar} className="star-icon" />
                                    </div>
                                    <div className="rating">
                                        <span>Service Kindness: {restaurant.serviceKindness ? restaurant.serviceKindness.toFixed(1) : 'N/A'}</span>
                                        <FontAwesomeIcon icon={faStar} className="star-icon" />
                                    </div>
                                    <div className="button-container">
                                        <button className="delete-button" onClick={() => handleDeleteRestaurant(restaurant.id)}>
                                            <FontAwesomeIcon icon={faTrashAlt} />
                                        </button>
                                    </div>
                                </div>
                                {selectedRestaurant && selectedRestaurant.id === restaurant.id && (
                                    <div className="restaurant-details">
                                        <p>Categorie: {selectedRestaurant.categoryName}</p>
                                        <div className="location-list-container">
                                            <ul className="location-list">
                                                <h3>Locații:</h3>
                                                {selectedRestaurant.locations && selectedRestaurant.locations.map((location) => (
                                                    <li
                                                        key={location.id}
                                                        onMouseEnter={() => setHighlightedLocationId(location.id)}
                                                        onMouseLeave={() => setHighlightedLocationId(null)}
                                                    >
                                                        {location.street}, {location.city}, {location.country} (Lat: {location.latitude}, Long: {location.longitude})
                                                    </li>
                                                ))}
                                            </ul>
                                            <MapContainer center={[44.4268, 26.1025]} zoom={13} style={{ height: "300px", width: "100%" }}>
                                                <TileLayer
                                                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                                                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                                                />
                                                {selectedRestaurant.locations && selectedRestaurant.locations.map((location) => (
                                                    <Marker
                                                        key={location.id}
                                                        position={[location.latitude, location.longitude]}
                                                        icon={highlightedLocationId === location.id ? highlightedIcon : customIcon}
                                                    ></Marker>
                                                ))}
                                            </MapContainer>
                                        </div>
                                    </div>
                                )}
                            </li>
                        ))}
                    </ul>
                ) : (
                    <p>Nu ai adăugat încă niciun restaurant la favorite.</p>
                )}
            </div>
            <div className="recommended-restaurants">
                <h1>Recomandări de restaurante</h1>
                {recommendedRestaurants.length > 0 ? (
                    <ul>
                        {recommendedRestaurants.map((restaurant) => (
                            <li key={restaurant.id}>
                                <div className="restaurant-info">
                                    <div className={`recommended-restaurant-logo ${getRestaurantLogoClass(restaurant.name)}`}></div>
                                    <h2>{restaurant.name}</h2>
                                    <div className="rating">
                                        <span>Food Quality: {restaurant.foodQuality ? restaurant.foodQuality.toFixed(1) : 'N/A'}</span>
                                        <FontAwesomeIcon icon={faStar} className="star-icon" />
                                    </div>
                                    <div className="rating">
                                        <span>Service Kindness: {restaurant.serviceKindness ? restaurant.serviceKindness.toFixed(1) : 'N/A'}</span>
                                        <FontAwesomeIcon icon={faStar} className="star-icon" />
                                    </div>
                                    <button className="small-add-button" onClick={() => handleAddFavoriteRestaurant(restaurant.name)}>
                                        +
                                    </button>
                                </div>
                            </li>
                        ))}
                    </ul>
                ) : (
                    <p>Nu există recomandări disponibile.</p>
                )}
            </div>
        </div>
    );
}

const getRestaurantLogoClass = (name) => {
    if (!name) return 'restaurant-logo';

    let className = 'restaurant-logo';
    switch (name.toLowerCase()) {
        case 'mcdonalds':
            className += ' mcdonalds';
            break;
        case 'kfc':
            className += ' kfc';
            break;
        case 'taverna racilor':
            className += ' taverna';
            break;
        case 'la italianu veteranilor':
            className += ' laitalianuveteranilor';
            break;
        case 'zest pizza':
            className += ' zest';
            break;
        case 'shift':
            className += ' shift';
            break;
        case 'la maria si ion':
            className += ' lamariasiion';
            break;
        case 'hanu berarilor':
            className += ' hanuberarilor';
            break;
        case 'dristor':
            className += ' dristor';
            break;
        default:
            className += ' default';
            break;
    }
    return className;
};

export default FavoriteRestaurants;
