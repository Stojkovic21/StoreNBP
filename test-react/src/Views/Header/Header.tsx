import { Link, NavLink } from "react-router-dom";
import "./Header.css";
import "../style/Visibility.css";
import useAuth from "../../hooks/useAuth";
import axios from "../../api/axios";
import Cart from "./CartIcon";
import Profile from "./ProfileIcon";
export default function main() {
  const { isAuthenticated, handleSignOut } = useAuth();
  return (
    <>
      <header className="main-header">
        <div className="logo">
          <NavLink
            className="link-offset-2 link-secondary link-underline link-underline-opacity-0"
            to={"/"}
          >
            MySite
          </NavLink>
        </div>
        <nav className="nav-links">
          <Link to="/newitem">New item</Link>
            {isAuthenticated ? (
            <>
              <Link to={`/mybills`}>My Bills</Link>
              <Link
                to="/login"
                onClick={async () => {
                  handleSignOut();
                  axios.get("/customer/signout", { withCredentials: true });
                }}
              >
                SignOut
              </Link>
            </>
            ) : (
            <>
              <Link to="/login">Login</Link>
              <Link to="/signup">Sign In</Link>
            </>
            )}
        </nav>
        <>
          <Profile />
          <Cart />
        </>
      </header>
    </>
  );
}
