import axios from "../api/axios";
import useAuth from "./useAuth";

const useRefreshToken = () => {
  const { handleSignIn } = useAuth();

  const refresh = async () => {
    const response = await axios.get("customer/refresh-token", {
      withCredentials: true,
    });
    handleSignIn(
      response.data.accessToken,
      response.data.customerId,
      response.data.role
    );
    return response.data.accessToken;
  };
  return refresh;
};

export default useRefreshToken;
