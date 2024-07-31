import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import './../css/FavoriteRestaurants.css';
import './../css/Profile.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheck, faStar, faArrowUp } from '@fortawesome/free-solid-svg-icons';

function Profile() {
    const [profileData, setProfileData] = useState(null);
    const [isFriend, setIsFriend] = useState(false);
    const [loggedInUserId, setLoggedInUserId] = useState(null);
    const [favoriteRestaurants, setFavoriteRestaurants] = useState([]);
    const [myFavoriteRestaurants, setMyFavoriteRestaurants] = useState([]);
    const [categories, setCategories] = useState([]);
    const [selectedRestaurant, setSelectedRestaurant] = useState(null);
    const [showFavoriteRestaurants, setShowFavoriteRestaurants] = useState(false);
    let { firstName, lastName } = useParams();

    const fetchProfileData = (firstName, lastName, token) => {
        console.log(`Fetching profile data for: ${firstName} ${lastName}`);
        axios.get(`https://localhost:7262/Auth/getProfile/${firstName}/${lastName}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then((profileResponse) => {
                const profile = profileResponse.data;
                console.log('Profile data:', profile);
                setProfileData(profile);

                axios.get(`https://localhost:7262/api/Person/${profile.firstName}/${profile.lastName}/friends`, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                })
                    .then((friendResponse) => {
                        const friends = friendResponse.data;
                        console.log('Friends:', friends);
                        const isFriend = friends.some(friend => friend.id === loggedInUserId);
                        console.log('Is friend:', isFriend);
                        setIsFriend(isFriend);

                        if (isFriend) {
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

                            axios.get(`https://localhost:7262/Auth/getFoodPlaces`, {
                                headers: {
                                    'Authorization': `Bearer ${token}`
                                }
                            })
                                .then(response => {
                                    setMyFavoriteRestaurants(response.data);
                                })
                                .catch(error => {
                                    console.error('A apărut o eroare la obținerea restaurantelor favorite ale utilizatorului logat.', error);
                                });
                        }
                    })
                    .catch((error) => {
                        console.error(`There was an error checking the friendship status: ${error}`);
                    });
            })
            .catch((error) => {
                console.error(`There was an error retrieving the profile data: ${error}`);
            });
    };

    useEffect(() => {
        const token = localStorage.getItem('token');
        const storedFirstName = localStorage.getItem('firstName');
        const storedLastName = localStorage.getItem('lastName');

        if (!firstName || !lastName) {
            firstName = storedFirstName;
            lastName = storedLastName;
        }

        if (firstName && lastName) {
            axios.get(`https://localhost:7262/api/Person/${storedLastName}/${storedFirstName}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            })
                .then((response) => {
                    const loggedInUser = response.data;
                    console.log('Logged in user data:', loggedInUser);
                    setLoggedInUserId(loggedInUser.id);
                    fetchProfileData(firstName, lastName, token);
                })
                .catch((error) => {
                    console.error(`There was an error retrieving the logged-in user data: ${error}`);
                });
        }
    }, [firstName, lastName]);

    const handleAddFriend = () => {
        const token = localStorage.getItem('token');

        if (loggedInUserId && profileData && profileData.personId) {
            axios.post(`https://localhost:7262/api/Person/${loggedInUserId}/friends/${profileData.personId}`, {}, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            })
                .then(() => {
                    console.log('Friend added successfully.');
                    setIsFriend(true);
                    fetchProfileData(profileData.firstName, profileData.lastName, token);
                })
                .catch((error) => {
                    console.error(`There was an error adding the friend: ${error}`);
                });
        } else {
            console.error('User IDs are not set correctly.');
        }
    };

    const handleShowFavoriteRestaurants = () => {
        const token = localStorage.getItem('token');
        axios.get(`https://localhost:7262/api/Person/${profileData.personId}/foodPlaces`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then((response) => {
                const favoriteRestaurants = response.data;
                console.log('Favorite restaurants:', favoriteRestaurants);
                const fetchRatingsPromises = favoriteRestaurants.map(restaurant =>
                    Promise.all([
                        fetchFoodQuality(restaurant.name),
                        fetchServiceKindness(restaurant.name)
                    ]).then(([foodQuality, serviceKindness]) => ({
                        ...restaurant,
                        foodQuality,
                        serviceKindness
                    }))
                );
                Promise.all(fetchRatingsPromises).then(setFavoriteRestaurants);
                setShowFavoriteRestaurants(true);
            })
            .catch((error) => {
                console.error(`There was an error retrieving the favorite restaurants: ${error}`);
            });
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

    const handleRestaurantHover = (restaurant) => {
        const category = categories.find(cat => cat.id === restaurant.foodPlaceCategoryId);
        setSelectedRestaurant({ ...restaurant, categoryName: category ? category.name : 'N/A' });
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

    const isRestaurantInMyFavorites = (restaurantName) => {
        return myFavoriteRestaurants.some(restaurant => restaurant.name === restaurantName);
    };

    const handlePromoteUser = () => {
        const token = localStorage.getItem('token');

        if (profileData && profileData.personId) {
            axios.post(`https://localhost:7262/Auth/makeAdmin/${profileData.personId}`, {}, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            })
                .then(() => {
                    alert('Utilizatorul a fost promovat.');
                    fetchProfileData(profileData.firstName, profileData.lastName, token); // Fetch the updated profile data
                })
                .catch((error) => {
                    console.error(`There was an error promoting the user: ${error}`);
                });
        } else {
            console.error('Profile data is not set correctly.');
        }
    };

    if (!profileData) {
        return <div>Loading...</div>;
    }

    return (
        <div className="profile-page">
            <div className="profile-background"></div>
            <div className="profile-container">
                <div className="profile-content">
                    <h1>Profil</h1>
                    <div className="profile-card">
                        <div className="profile-card-title">Informații de bază</div>
                        <div className="profile-card-value">
                            <p>Nume de utilizator: {profileData.userName}</p>
                            <p>Email: {profileData.email}</p>
                        </div>
                    </div>
                    <div className="profile-card">
                        <div className="profile-card-title">Rol</div>
                        <div className="profile-card-value">
                            {profileData.role}
                            {profileData.role !== 'Admin' && (
                                <button className="upgrade-button" onClick={handlePromoteUser}>
                                    <FontAwesomeIcon icon={faArrowUp} />
                                </button>
                            )}
                        </div>
                    </div>
                    <div className="profile-card">
                        <div className="profile-card-title">Nume</div>
                        <div className="profile-card-value">{profileData.lastName}</div>
                    </div>
                    <div className="profile-card">
                        <div className="profile-card-title">Prenume</div>
                        <div className="profile-card-value">{profileData.firstName}</div>
                    </div>
                    {loggedInUserId !== profileData.personId && (
                        <>
                            <div className="profile-card friend-status">
                                <div className="profile-card-title">Prieteni</div>
                                <div className="profile-card-value">
                                    {isFriend ? (
                                        <p>Sunteți prieteni <FontAwesomeIcon icon={faCheck} /></p>
                                    ) : (
                                        <button className="favorite-restaurants-button" onClick={handleAddFriend}>Adaugă ca prieten</button>
                                    )}
                                </div>
                            </div>
                            {isFriend && (
                                <div className="profile-card">
                                    <div className="favorite-restaurants">
                                        <button className="favorite-restaurants-button" onClick={handleShowFavoriteRestaurants}>Vezi restaurantele favorite</button>
                                        {showFavoriteRestaurants && (
                                            <div className="favorite-restaurants">
                                                {favoriteRestaurants.length > 0 ? (
                                                    <ul>
                                                        {favoriteRestaurants.map((restaurant) => (
                                                            <li key={restaurant.id} onMouseOver={() => handleRestaurantHover(restaurant)} onMouseLeave={() => setSelectedRestaurant(null)}>
                                                                <div className="restaurant-info">
                                                                    <div className={`restaurant-logo ${getRestaurantLogoClass(restaurant.name)}`}></div>
                                                                    <h2>{restaurant.name}</h2>
                                                                    <div className="rating">
                                                                        <span>Food Quality: {restaurant.foodQuality ? restaurant.foodQuality.toFixed(1) : 'N/A'}</span>
                                                                        <FontAwesomeIcon icon={faStar} className="star-icon" />
                                                                    </div>
                                                                    <div className="rating">
                                                                        <span>Service Kindness: {restaurant.serviceKindness ? restaurant.serviceKindness.toFixed(1) : 'N/A'}</span>
                                                                        <FontAwesomeIcon icon={faStar} className="star-icon" />
                                                                    </div>
                                                                    <div className="restaurant-actions">
                                                                        {isRestaurantInMyFavorites(restaurant.name) ? (
                                                                            <span className="favorite-check">
                                                                                <FontAwesomeIcon icon={faCheck} />
                                                                            </span>
                                                                        ) : (
                                                                            <span className="add-favorite-icon" onClick={() => handleAddFavoriteRestaurant(restaurant.name)}>+</span>
                                                                        )}
                                                                    </div>
                                                                </div>
                                                                {selectedRestaurant && selectedRestaurant.id === restaurant.id && (
                                                                    <div className="restaurant-details">
                                                                        <p>Categorie: {selectedRestaurant.categoryName}</p>
                                                                    </div>
                                                                )}
                                                            </li>
                                                        ))}
                                                    </ul>
                                                ) : (
                                                    <p>Prietenul tău nu are restaurante favorite.</p>
                                                )}
                                            </div>
                                        )}
                                    </div>
                                </div>
                            )}
                        </>
                    )}
                </div>
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

export default Profile;
