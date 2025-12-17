import { useForm } from "react-hook-form";
import "../style/Card.css";
import customerDto from "../../DTOs/CustomerDto";
import Header from "../Header/Header";
import axios from "../../api/axios";
import { useNavigate } from "react-router-dom";
import useAuth from "../../hooks/useAuth";

function SignUp() {
  const navigate = useNavigate();
  const {handleSignIn}=useAuth();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<customerDto>({mode:"onBlur"});

  const onSubmit = async (data: customerDto) => {
    data.role = "Customer";
    console.log("Data ", data);
    await axios.post("/customer/signup", data,{
        withCredentials: true,
      })
      .then((response) => {
        handleSignIn(
          response.data.accessToken,
          response.data.userId,
          response.data.role
        );
        navigate("/");
        return response.status;
      })
      .catch((error) => {
        if (error.response.status === 400) {
          console.log("Korisnik sa tim emailom vec postoji");
        }
      });
    await new Promise((responce) => setTimeout(responce, 1000));
  };

  return (
    <>
      <Header />
      <div className="card-container">
        <div className="card">
          <h2>User Registration</h2>
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="mb-3">
              <label className="form-label">Name</label>
              <input
                type="text"
                className="form-control"
                placeholder="Enter your Name"
                {...register("name", { required: "Name is required",minLength:{
                  value:2,
                  message:"The name has to have least than 2 letters"
                } })}
              />
              {errors.name && (
                <span id="name-error-msg"className="text-danger">*{errors.name?.message}</span>
              )}
            </div>
            <div className="mb-3">
              <label className="form-label">Lastname</label>
              <input
                type="text"
                className="form-control"
                placeholder="Enter yout Lastname"
                {...register("lastname", { required: "Lastname is required",minLength:{
                  value:5,
                  message:"The lastname has to have least than 5 letters"
                } })}
              />
              {errors.lastname && (
                <span id="lastname-error-msg" className="text-danger">*{errors.lastname?.message}</span>
              )}
            </div>
            <div className="mb-3">
              <label className="form-label">Email</label>
              <input
                className="form-control"
                type="email"
                placeholder="Enter your Email"
                {...register("email", {
                  required: "Email is required",
                  pattern: {
                    value: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
                    message: "Invalid email address",
                  },
                })}
              />
              {errors.email && (
                <span id="email-error-msg" className="text-danger">*{errors.email?.message}</span>
              )}
            </div>
            <div className="mb-3">
              <label className="form-label">Password</label>
              <input
                type="password"
                placeholder="Enter your password"
                className="form-control"
                {...register("password", {
                  required: "Password is required",
                  // pattern: {
                  //   value: /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/,
                  //   message:
                  //     "Password must be at least 8 characters long and include at least one big letter and number",
                  //},
                })}
              />
              {errors.password && (
                <span id="password-error-msg" className="text-danger">*{errors.password?.message}</span>
              )}
            </div>
            <div className="mb-3">
              <label className="form-label">Phone number</label>
              <input
                type="text"
                className="form-control"
                placeholder="Enter your phone number"
                {...register("phoneNumber", { required: "Phone number is required",minLength:{
                  value:9,
                  message:"Enter correct phone number"
                } })}
              />
              {errors.phoneNumber && (
                <span id="phone-error-msg" className="text-danger">*{errors.phoneNumber?.message}</span>
              )}
            </div>

            <button type="submit" className="btn w-100" disabled={isSubmitting}>
              {isSubmitting ? "Loading..." : "Submit"}
            </button>
          </form>
        </div>
      </div>
    </>
  );
}

export default SignUp;
