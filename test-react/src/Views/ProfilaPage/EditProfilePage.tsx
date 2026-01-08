import { SubmitHandler, useForm } from "react-hook-form";
import "../style/Card.css";
import axios from "../../api/axios";

type editProfile = {
  name?: string;
  lastName?: string;
  phoneNumber?: string;
  email?: string;
};
export default function EditProfilePage({
  name,
  lastName,
  email,
  phoneNumber,
}: editProfile) {
  const {
    register,
    handleSubmit,
    formState: { isSubmitting },
  } = useForm<editProfile>();
  const onSubmit: SubmitHandler<editProfile> = async (data) => {
    await axios.put(`customer/update`, data,{withCredentials:true}).then(() => {
      window.location.reload();
    });
  };
  return (
    <>
      <div className="profile-container">
        <form onSubmit={handleSubmit(onSubmit)} className="profile-card">
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
            <h2 className="profile-name">
              {name} {lastName}
            </h2>
            <p className="profile-email">{email}</p>

            <div className="profile-info-list">
              <div className="info-item">
                <span className="info-label">First Name</span>
                <input
                  type="name"
                  defaultValue={name}
                  className="form-control"
                  placeholder="Enter your name"
                  {...register("name", {
                    required: "Name is required",
                  })}
                />
              </div>

              <div className="info-item">
                <span className="info-label">Last Name</span>
                <input
                  type="lastName"
                  defaultValue={lastName}
                  className="form-control"
                  placeholder="Enter your last name"
                  {...register("lastName", {
                    required: "Last name is required",
                  })}
                />
              </div>

              <div className="info-item">
                <span className="info-label">Phone Number</span>
                <input
                  type="phone number"
                  defaultValue={phoneNumber}
                  className="form-control"
                  placeholder="Enter your phone number"
                  {...register("phoneNumber", {
                    required: "Phone number is required",
                  })}
                />
              </div>
            </div>

            <button
              type="submit"
              className="edit-button"
              disabled={isSubmitting}
            >
              {isSubmitting ? "Loading..." : "Submit"}
            </button>
          </div>
        </form>
      </div>
    </>
  );
}
