import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './../css/Friends.css';
import FriendSearchBar from './FriendSearchBar';

function Friends() {
    const [friends, setFriends] = useState([]);
    const [searchQuery, setSearchQuery] = useState('');
    const [reviewsCount, setReviewsCount] = useState({});
    const navigate = useNavigate();

    useEffect(() => {
        const firstName = localStorage.getItem('firstName');
        const lastName = localStorage.getItem('lastName');
        const token = localStorage.getItem('token');

        axios.get(`https://localhost:7262/api/Person/${firstName}/${lastName}/friends`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then((response) => {
                setFriends(response.data);
                response.data.forEach(friend => {
                    if (friend.id) {
                        fetchReviewsCount(friend.id);
                    } else {
                        console.error('Friend personId is undefined', friend);
                    }
                });
            })
            .catch((error) => {
                console.error(`There was an error retrieving the friends data: ${error}`);
            });
    }, []);

    const fetchReviewsCount = (personId) => {
        const token = localStorage.getItem('token');
        axios.get(`https://localhost:7262/api/CustomerReview/person/${personId}/numberOfReviews`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then((response) => {
                setReviewsCount(prevState => ({
                    ...prevState,
                    [personId]: response.data
                }));
            })
            .catch((error) => {
                console.error(`There was an error retrieving the review count: ${error}`);
            });
    };

    if (!friends) {
        return <div>Loading...</div>;
    }

    const handleFriendClick = (firstName, lastName) => {
        localStorage.setItem('searchFirstName', firstName);
        localStorage.setItem('searchLastName', lastName);
        navigate(`/profile/${firstName}/${lastName}`);
    };

    const filteredFriends = friends.filter(friend =>
        friend.firstName.toLowerCase().includes(searchQuery.toLowerCase()) ||
        friend.lastName.toLowerCase().includes(searchQuery.toLowerCase())
    );

    return (
        <div className="friends-page">
            <div className="friends-container">
                <h1 className="text-center my-4">Prietenii mei</h1>
                <div className="friend-search-bar mb-4">
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Cautare"
                        value={searchQuery}
                        onChange={e => setSearchQuery(e.target.value)}
                    />
                </div>
                <div className="list-group">
                    {filteredFriends.map((friend, index) => (
                        <div
                            key={index}
                            className="list-group-item list-group-item-action friend"
                            onClick={() => handleFriendClick(friend.firstName, friend.lastName)}
                        >
                            <h5 className="mb-1">{friend.firstName} {friend.lastName}</h5>
                            <div className="friend-details">
                                <p className="mb-1">Email: {friend.emailAddress}</p>
                                <p className="mb-1">Număr de recenzii: {reviewsCount[friend.id] !== undefined ? reviewsCount[friend.id] : 'Loading...'}</p>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}

export default Friends;
