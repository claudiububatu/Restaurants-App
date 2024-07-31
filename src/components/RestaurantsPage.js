import React, { useEffect, useState, useRef } from 'react';
import axios from 'axios';
import Slider from 'react-slick';
import './../css/RestaurantsPage.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheck, faChevronLeft, faChevronRight, faStar, faPlus, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import 'slick-carousel/slick/slick.css';
import 'slick-carousel/slick/slick-theme.css';
import { MapContainer, TileLayer, Marker, useMapEvents } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import { GeoSearchControl, OpenStreetMapProvider } from 'leaflet-geosearch';
import L from 'leaflet';
import StarRatingComponent from 'react-star-rating-component';
import Modal from 'react-modal';

const provider = new OpenStreetMapProvider();

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

function LocationMarker({ setNewLocation }) {
    const map = useMapEvents({
        click(e) {
            const { lat, lng } = e.latlng;
            setNewLocation(prev => ({ ...prev, latitude: lat, longitude: lng }));
            fetchAddress(lat, lng, setNewLocation);
        }
    });

    return null;
}

const fetchAddress = (latitude, longitude, setNewLocation) => {
    provider.search({ query: `${latitude}, ${longitude}` })
        .then((result) => {
            if (result.length > 0) {
                console.log("Geocoder result:", result);  // Log the entire result for debugging
                const labelParts = result[0].label.split(', ');
                console.log("Label parts:", labelParts);  // Log the label parts

                let street = '';
                let city = '';
                let country = '';

                // Check for specific keywords to correctly assign street, city, and country
                labelParts.forEach(part => {
                    if (/bulevardul|strada|calea|aleea|drumul/i.test(part)) {
                        street = part;
                    } else if (/sector|bucurești|cluj|timisoara|constanta/i.test(part)) {
                        city = part;
                    } else if (/românia/i.test(part)) {
                        country = part;
                    }
                });

                // Fallbacks in case the initial parsing fails
                if (!street) {
                    street = labelParts.length > 1 ? labelParts[0] : '';
                }
                if (!city) {
                    city = labelParts.length > 2 ? labelParts[labelParts.length - 3] : '';
                }
                if (!country) {
                    country = labelParts.length > 0 ? labelParts[labelParts.length - 1] : '';
                }

                setNewLocation(prev => ({
                    ...prev,
                    street: street,
                    city: city,
                    country: country
                }));
            }
        })
        .catch(error => {
            console.error('A apărut o eroare la obținerea adresei.', error);
        });
};

function RestaurantsPage() {
    const [restaurants, setRestaurants] = useState([]);
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('All');
    const [myFavoriteRestaurants, setMyFavoriteRestaurants] = useState([]);
    const [locations, setLocations] = useState([]);
    const [newLocation, setNewLocation] = useState({
        street: '',
        city: '',
        country: '',
        foodPlaceId: 0,
        latitude: 0,
        longitude: 0
    });
    const [newReview, setNewReview] = useState({
        title: '',
        text: '',
        serviceKindness: 0,
        foodQuality: 0
    });
    const [reviews, setReviews] = useState([]);
    const [loggedInUserId, setLoggedInUserId] = useState(null);
    const [userRole, setUserRole] = useState('');
    const [selectedRestaurantId, setSelectedRestaurantId] = useState(null);
    const [selectedRestaurantName, setSelectedRestaurantName] = useState('');
    const [highlightedLocationId, setHighlightedLocationId] = useState(null);
    const [foodQuality, setFoodQuality] = useState(null);
    const [serviceKindness, setServiceKindness] = useState(null);
    const [newRestaurantName, setNewRestaurantName] = useState('');
    const [newRestaurantCategoryId, setNewRestaurantCategoryId] = useState('');
    const [newRestaurantImageUrl, setNewRestaurantImageUrl] = useState('');
    const [refresh, setRefresh] = useState(false);
    const sliderRef = useRef(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    useEffect(() => {
        axios.get('https://localhost:7262/api/FoodPlaceCategory')
            .then(response => {
                setCategories(response.data);
            })
            .catch(error => {
                console.error('A apărut o eroare la obținerea categoriilor.', error);
            });

        fetchRestaurants('All');

        const token = localStorage.getItem('token');
        if (token) {
            axios.get('https://localhost:7262/Auth/getProfile', {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            })
                .then(response => {
                    setLoggedInUserId(response.data.personId);
                    setUserRole(response.data.role);
                    axios.get('https://localhost:7262/Auth/getFoodPlaces', {
                        headers: {
                            'Authorization': `Bearer ${token}`
                        }
                    })
                        .then(response => {
                            setMyFavoriteRestaurants(response.data);
                        })
                        .catch(error => {
                            console.error('A apărut o eroare la obținerea restaurantelor favorite.', error);
                        });
                })
                .catch(error => {
                    console.error('A apărut o eroare la obținerea profilului utilizatorului.', error);
                });
        }
    }, [refresh]);

    useEffect(() => {
        if (restaurants.length > 0) {
            const firstRestaurant = restaurants[0];
            setSelectedRestaurantId(firstRestaurant.id);
            setSelectedRestaurantName(firstRestaurant.name);
            fetchLocations(firstRestaurant.name);
            fetchReviews(firstRestaurant.id);

            // Fetch ratings
            fetchFoodQuality(firstRestaurant.id).then(setFoodQuality);
            fetchServiceKindness(firstRestaurant.id).then(setServiceKindness);
        }
    }, [restaurants]);

    const handleRestaurantChange = (restaurant) => {
        setSelectedRestaurantId(restaurant.id);
        setSelectedRestaurantName(restaurant.name);
        fetchLocations(restaurant.name);
        fetchReviews(restaurant.id);

        // Fetch ratings
        fetchFoodQuality(restaurant.id).then(setFoodQuality);
        fetchServiceKindness(restaurant.id).then(setServiceKindness);
    };

    const fetchRestaurants = (category) => {
        let url = 'https://localhost:7262/api/FoodPlace';

        if (category !== 'All') {
            url = `https://localhost:7262/api/FoodPlaceCategory/${category}/foodPlaces`;
        }

        axios.get(url)
            .then(response => {
                const filteredRestaurants = response.data.filter(restaurant => restaurant.personId === 5);
                setRestaurants(filteredRestaurants);
            })
            .catch(error => {
                console.error('A apărut o eroare la obținerea restaurantelor.', error);
            });
    };

    const fetchLocations = (foodPlaceName) => {
        console.log(`Fetching locations for foodPlaceName: ${foodPlaceName}`);
        axios.get(`https://localhost:7262/api/FoodPlace/${foodPlaceName}/locations`)
            .then(response => {
                setLocations(response.data);
            })
            .catch(error => {
                console.error('A apărut o eroare la obținerea locațiilor.', error);
            });
    };

    const fetchReviews = (foodPlaceId) => {
        console.log(`Fetching reviews for foodPlaceId: ${foodPlaceId}`);
        axios.get(`https://localhost:7262/api/CustomerReview/foodplace/${foodPlaceId}`)
            .then(response => {
                const reviewsWithPersonDetails = response.data.map(review => {
                    return axios.get(`https://localhost:7262/api/Person/${review.personId}`)
                        .then(personResponse => {
                            return { ...review, person: personResponse.data };
                        })
                        .catch(error => {
                            console.error('A apărut o eroare la obținerea detaliilor persoanei.', error);
                            return { ...review, person: { firstName: 'N/A', lastName: '' } }; // Fallback in case of error
                        });
                });
                Promise.all(reviewsWithPersonDetails)
                    .then(fullReviews => {
                        setReviews(fullReviews);
                    });
            })
            .catch(error => {
                console.error('A apărut o eroare la obținerea recenziilor.', error);
            });
    };

    const fetchFoodQuality = (foodPlaceId) => {
        return axios.get(`https://localhost:7262/api/CustomerReview/foodQuality/${foodPlaceId}`)
            .then(response => response.data)
            .catch(error => {
                console.error('A apărut o eroare la obținerea calității mâncării.', error);
                return null;
            });
    };

    const fetchServiceKindness = (foodPlaceId) => {
        return axios.get(`https://localhost:7262/api/CustomerReview/serviceKindness/${foodPlaceId}`)
            .then(response => response.data)
            .catch(error => {
                console.error('A apărut o eroare la obținerea amabilității serviciului.', error);
                return null;
            });
    };

    const handleCategoryChange = (e) => {
        const category = e.target.value;
        setSelectedCategory(category);
        fetchRestaurants(category);
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
                    personId: loggedInUserId,
                    ImageUrl: './../images/restaurants/default.jpg'
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
                                setMyFavoriteRestaurants(response.data);
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

    const handleAddLocation = (e) => {
        e.preventDefault();
        const token = localStorage.getItem('token');
        const updatedLocation = { ...newLocation, foodPlaceId: selectedRestaurantId };

        // Check if selectedRestaurantId is set correctly
        console.log("selectedRestaurantId:", selectedRestaurantId);
        console.log("selectedRestaurantName:", selectedRestaurantName);

        if (!selectedRestaurantId) {
            console.error("FoodPlaceId is not set. Cannot add location.");
            alert("Please select a restaurant before adding a location.");
            return;
        }

        console.log("Submitting new location:", updatedLocation);

        axios.post('https://localhost:7262/api/Location', updatedLocation, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        })
            .then(() => {
                alert('Locația a fost adăugată cu succes.');
                fetchLocations(selectedRestaurantName);
                setNewLocation({
                    street: '',
                    city: '',
                    country: '',
                    foodPlaceId: 0,
                    latitude: 0,
                    longitude: 0
                });
            })
            .catch(error => {
                console.error('A apărut o eroare la adăugarea locației.', error);
            });
    };

    const handleAddReview = (e) => {
        e.preventDefault();
        const token = localStorage.getItem('token');
        const updatedReview = { ...newReview, foodPlaceId: selectedRestaurantId, personId: loggedInUserId };

        console.log("Submitting new review:", updatedReview);

        axios.post('https://localhost:7262/api/CustomerReview', updatedReview, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        })
            .then(() => {
                alert('Recenzia a fost adăugată cu succes.');
                fetchReviews(selectedRestaurantId);
                fetchFoodQuality(selectedRestaurantId).then(setFoodQuality);
                fetchServiceKindness(selectedRestaurantId).then(setServiceKindness);
                setNewReview({
                    title: '',
                    text: '',
                    serviceKindness: 0,
                    foodQuality: 0
                });
            })
            .catch(error => {
                console.error('A apărut o eroare la adăugarea recenziei.', error);
            });
    };

    const isRestaurantInMyFavorites = (restaurantName) => {
        return myFavoriteRestaurants.some(restaurant => restaurant.name === restaurantName);
    };

    const getRestaurantLogoClass = (name) => {
        if (!name) {
            name = 'default';
        }

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

    const handleAddRestaurant = () => {
        const token = localStorage.getItem('token');
        const defaultImagePath = './../images/restaurants/default.jpg';

        axios.post('https://localhost:7262/api/FoodPlace', {
            name: newRestaurantName,
            foodPlaceCategoryId: newRestaurantCategoryId,
            ImageUrl: defaultImagePath  // Set the default image path
        }, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => {
                setRestaurants([...restaurants, response.data]);
                setNewRestaurantName('');
                setNewRestaurantCategoryId('');
                setNewRestaurantImageUrl('./../images/restaurants/default.jpg');
                console.log("SETEZ CAMPURILE UNUI RESTAURANT NOU ADAUGAT");
                setRefresh(!refresh); // Inversează valoarea pentru a forța reîmprospătarea
                sliderRef.current.slickGoTo(0); // Mută slider-ul pe primul slide
                setIsModalOpen(false); // Close the modal after adding the restaurant
            })
            .catch(error => {
                console.error('A apărut o eroare la adăugarea restaurantului.', error);
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

    const sliderSettings = {
        dots: true,
        infinite: false,
        speed: 500,
        slidesToShow: 1,
        slidesToScroll: 1,
        nextArrow: <SampleNextArrow />,
        prevArrow: <SamplePrevArrow />,
        adaptiveHeight: true,
        afterChange: (index) => {
            if (restaurants[index]) {
                handleRestaurantChange(restaurants[index]);
            }
        }
    };

    function SampleNextArrow(props) {
        const { className, style, onClick } = props;
        return (
            <div
                className="custom-arrow next-arrow"
                style={{ ...style }}
                onClick={onClick}
            >
                <FontAwesomeIcon icon={faChevronRight} />
            </div>
        );
    }

    function SamplePrevArrow(props) {
        const { className, style, onClick } = props;
        return (
            <div
                className="custom-arrow prev-arrow"
                style={{ ...style }}
                onClick={onClick}
            >
                <FontAwesomeIcon icon={faChevronLeft} />
            </div>
        );
    }

    return (
        <div className="restaurants-page">
            <h1>Restaurantele noastre</h1>
            <div className="filter-container">
                <label htmlFor="category-select">Alege o categorie:</label>
                <select id="category-select" value={selectedCategory} onChange={handleCategoryChange}>
                    <option value="All">Toate</option>
                    {categories.map(category => (
                        <option key={category.id} value={category.name}>{category.name}</option>
                    ))}
                </select>
            </div>
            {loggedInUserId === 5 && (
                <button className="floating-button" onClick={() => setIsModalOpen(true)}>
                    <FontAwesomeIcon icon={faPlus} />
                </button>
            )}
            <Slider ref={sliderRef} {...sliderSettings} className="restaurants-slider">
                {restaurants.length > 0 ? (
                    restaurants.map((restaurant) => (
                        <div key={restaurant.id} className="restaurant-page">
                            <div className="restaurant-info">
                                <div className={getRestaurantLogoClass(restaurant.name)}></div>
                                <h2>{restaurant.name}</h2>
                                <div className="restaurant-ratings">
                                    <div className="rating">
                                        <FontAwesomeIcon icon={faStar} className="star-icon" />
                                        <span>Food Quality: {foodQuality ? foodQuality.toFixed(1) : 'N/A'}</span>
                                    </div>
                                    <div className="rating">
                                        <FontAwesomeIcon icon={faStar} className="star-icon" />
                                        <span>Service Kindness: {serviceKindness ? serviceKindness.toFixed(1) : 'N/A'}</span>
                                    </div>
                                </div>
                                <div className="restaurant-actions">
                                    {loggedInUserId && isRestaurantInMyFavorites(restaurant.name) ? (
                                        <span className="favorite-check">
                                            <FontAwesomeIcon icon={faCheck} />
                                        </span>
                                    ) : (
                                        loggedInUserId && <span className="add-favorite-icon" onClick={() => handleAddFavoriteRestaurant(restaurant.name)}>+</span>
                                    )}
                                    {userRole === 'Admin' && <button className="delete-button" onClick={() => handleDeleteRestaurant(restaurant.id)}>
                                        <FontAwesomeIcon icon={faTrashAlt} />
                                    </button>}
                                </div>

                            </div>

                            <div className="restaurant-locations">
                                <h3>Locații:</h3>
                                {locations.length > 0 ? (
                                    <ul className="location-list">
                                        {locations.map((location) => (
                                            <li
                                                key={location.id}
                                                onMouseEnter={() => setHighlightedLocationId(location.id)}
                                                onMouseLeave={() => setHighlightedLocationId(null)}
                                                style={{ cursor: 'pointer' }}
                                            >
                                                {location.street}, {location.city}, {location.country} (Lat: {location.latitude}, Long: {location.longitude})
                                            </li>
                                        ))}
                                    </ul>
                                ) : (
                                    <p>Nu există locații disponibile.</p>
                                )}
                            </div>
                            {userRole === 'Admin' && (
                                <div className="restaurant-admin-actions">
                                    <form className="add-location-form" onSubmit={handleAddLocation}>
                                        <h3>Adaugă o locație nouă</h3>
                                        <input
                                            type="text"
                                            placeholder="Strada"
                                            value={newLocation.street}
                                            onChange={(e) => setNewLocation({ ...newLocation, street: e.target.value })}
                                            required
                                        />
                                        <input
                                            type="text"
                                            placeholder="Oraș"
                                            value={newLocation.city}
                                            onChange={(e) => setNewLocation({ ...newLocation, city: e.target.value })}
                                            required
                                        />
                                        <input
                                            type="text"
                                            placeholder="Țară"
                                            value={newLocation.country}
                                            onChange={(e) => setNewLocation({ ...newLocation, country: e.target.value })}
                                            required
                                        />
                                        <input
                                            type="number"
                                            placeholder="Latitudine"
                                            value={newLocation.latitude}
                                            onChange={(e) => setNewLocation({ ...newLocation, latitude: parseFloat(e.target.value) })}
                                            required
                                        />
                                        <input
                                            type="number"
                                            placeholder="Longitudine"
                                            value={newLocation.longitude}
                                            onChange={(e) => setNewLocation({ ...newLocation, longitude: parseFloat(e.target.value) })}
                                            required
                                        />
                                        <button type="submit">Adaugă locația</button>
                                    </form>
                                </div>
                            )}
                        </div>
                    ))
                ) : (
                    <p>Nu există restaurante disponibile.</p>
                )}
            </Slider>
            <div className="map-container">
                <MapContainer center={[44.4268, 26.1025]} zoom={13} style={{ height: "100%", width: "100%" }}>
                    <TileLayer
                        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    />
                    <LocationMarker setNewLocation={setNewLocation} />
                    {locations.map((location) => (
                        <Marker
                            key={location.id}
                            position={[location.latitude, location.longitude]}
                            icon={highlightedLocationId === location.id ? highlightedIcon : customIcon}
                        ></Marker>
                    ))}
                    {newLocation.latitude !== 0 && newLocation.longitude !== 0 && (
                        <Marker position={[newLocation.latitude, newLocation.longitude]} icon={customIcon}></Marker>
                    )}
                </MapContainer>
            </div>
            <div className="reviews-section">
                {loggedInUserId ? (
                    <form className="add-review-form" onSubmit={handleAddReview}>
                        <h3>Adaugă o recenzie</h3>
                        <input
                            type="text"
                            placeholder="Titlu"
                            value={newReview.title}
                            onChange={(e) => setNewReview({ ...newReview, title: e.target.value })}
                            required
                        />
                        <textarea
                            placeholder="Text"
                            value={newReview.text}
                            onChange={(e) => setNewReview({ ...newReview, text: e.target.value })}
                            required
                        />
                        <div>
                            <label>Amabilitatea serviciului:</label>
                            <StarRatingComponent
                                name="serviceKindness"
                                starCount={5}
                                value={newReview.serviceKindness}
                                onStarClick={(nextValue) => setNewReview({ ...newReview, serviceKindness: nextValue })}
                            />
                        </div>
                        <div>
                            <label>Calitatea mâncării:</label>
                            <StarRatingComponent
                                name="foodQuality"
                                starCount={5}
                                value={newReview.foodQuality}
                                onStarClick={(nextValue) => setNewReview({ ...newReview, foodQuality: nextValue })}
                            />
                        </div>
                        <button type="submit">Adaugă recenzia</button>
                    </form>
                ) : (
                    <p>Autentificați-vă pentru a adăuga o recenzie.</p>
                )}
                <div className="reviews-list">
                    <h3>Recenzii:</h3>
                    {reviews.length > 0 ? (
                        reviews.map((review) => (
                            <div key={review.id} className="review-item">
                                <div className="review-header">
                                    <p>{review.person.firstName} {review.person.lastName}</p>
                                </div>
                                <div className="review-content">
                                    <h4>{review.title}</h4>
                                    <p>{review.text}</p>
                                    <div>
                                        <label>Amabilitatea serviciului:</label>
                                        <StarRatingComponent
                                            name="serviceKindnessReadOnly"
                                            starCount={5}
                                            value={review.serviceKindness}
                                            editing={false}
                                        />
                                    </div>
                                    <div>
                                        <label>Calitatea mâncării:</label>
                                        <StarRatingComponent
                                            name="foodQualityReadOnly"
                                            starCount={5}
                                            value={review.foodQuality}
                                            editing={false}
                                        />
                                    </div>
                                </div>
                            </div>
                        ))
                    ) : (
                        <p>Nu există recenzii disponibile.</p>
                    )}
                </div>
            </div>
            <Modal
                isOpen={isModalOpen}
                onRequestClose={() => setIsModalOpen(false)}
                contentLabel="Add Restaurant Modal"
                className="Modal"
                overlayClassName="Overlay"
            >
                <h3>Adaugă un restaurant nou</h3>
                <input
                    type="text"
                    placeholder="Numele restaurantului"
                    value={newRestaurantName}
                    onChange={(e) => setNewRestaurantName(e.target.value)}
                    required
                />
                <select
                    value={newRestaurantCategoryId}
                    onChange={(e) => setNewRestaurantCategoryId(e.target.value)}
                    required
                >
                    <option value="">Selectează categoria</option>
                    {categories.map(category => (
                        <option key={category.id} value={category.id}>{category.name}</option>
                    ))}
                </select>
                <button onClick={handleAddRestaurant}>Adaugă restaurantul</button>
            </Modal>
        </div>
    );
}

export default RestaurantsPage;
