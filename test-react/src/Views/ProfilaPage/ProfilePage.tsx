import { useEffect, useState } from 'react';
import './ProfilePage.css';
import customerDto from '../../DTOs/CustomerDto';
import axios from '../../api/axios';
import useAuth from '../../hooks/useAuth';
import Header from '../Header/Header';
import EditProfilePage from './EditProfilePage';
import useRefreshToken from '../../hooks/useRefreshToken';

function ProfilePage() {
    const [customer, setCustomer]=useState<customerDto>();
    const {customerId,isAuthenticated}=useAuth();
    const refresh=useRefreshToken()
    const [isEditing,setIsEditing]=useState(false);
    useEffect(()=>{
        const fetchItems=async()=>{
            const result=await axios.get(`/customer/${customerId}`);
            setCustomer(result.data);
            setIsEditing(false);
        }
        if(!isAuthenticated)
            refresh();
        fetchItems();
        
    },[isAuthenticated]);
  return (
    <>
        <Header/>
        { isEditing==false &&
        <div className="profile-container">
        <div className="profile-card">
            {/* Header Background */}
            <div className="profile-header">
            <div className="image-container">
                {/* <img 
                src={user.picture} 
                alt={`${user.firstName} ${user.lastName}`} 
                className="profile-image" 
                /> */}
                Mesto za sliku
            </div>
            </div>

            <div className="profile-body">
            <h2 className="profile-name">{customer?.name} {customer?.lastName}</h2>
            <p className="profile-email">{customer?.email}</p>

            <div className="profile-info-list">
                <div className="info-item">
                <span className="info-label">First Name</span>
                <span className="info-value">{customer?.name}</span>
                </div>

                <div className="info-item">
                <span className="info-label">Last Name</span>
                <span className="info-value">{customer?.lastName}</span>
                </div>

                <div className="info-item">
                <span className="info-label">Email Address</span>
                <span className="info-value">{customer?.email}</span>
                </div>

                <div className="info-item">
                <span className="info-label">Phone Number</span>
                <span className="info-value">{customer?.phoneNumber}</span>
                </div>
            </div>

            <button className="edit-button" onClick={(()=>{setIsEditing(true)})}>Edit Profile</button>
            </div>
        </div>
        </div>}
        {
            isEditing==true && <EditProfilePage {...customer}/>
        }
    </>
  );
};

export default ProfilePage;