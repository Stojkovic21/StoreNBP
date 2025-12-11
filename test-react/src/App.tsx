import { createBrowserRouter, RouterProvider } from "react-router-dom";
import "./App.css";
import HomePage from "./Views/HomePage/HomePage";
import LoginPage from "./Views/Login/Login";
import SignUp from "./Views/Signup/SignUp";
import AddProduct from "./Views/AddProduct/AddProduct";
import { AuthProvider } from "./context/AuthContrext";
import { ShoppingCardProvider } from "./context/ShoppingCartContext";
import ItemProfile from "./Views/ProduktPage/ProduktPage";
import ProductCard from "./Views/ProduktPage/ProduktPage";

const router = createBrowserRouter([
  //razlika izmedju link i a je sto a refresuje ceo html i js a link samo prosledi na tu stranicu
  {
    path: "/",
    element: <HomePage />,
    errorElement: <div>404 not found</div>,
  },
  {
    path: "/login",
    element: <LoginPage />,
    errorElement: <div>404 not found</div>,
  },
  {
    path: "/signup",
    element: <SignUp />,
    errorElement: <div>404 not found</div>,
  },
  {
    path: "/product/:productid",
    element: <ProductCard/>,
    errorElement: <div>404 not found</div>
  },
  {
    path: "/newitem",
    element: <AddProduct />,
    errorElement: <div>404 not found</div>,
  },
  //profiles
  // {
  //   path:'profiles/:profileId',
  //   element: <ProfilePage>>
  // }
]);

function App() {
  return (
    <>
      <AuthProvider>
        <ShoppingCardProvider>
          <RouterProvider router={router} />
        </ShoppingCardProvider>
      </AuthProvider>
    </>
  );
}

export default App;
