import { Link } from "react-router-dom";
import productDto from "../../DTOs/ProductDto";
import "./Artikal.css";
import { axiosPrivate } from "../../api/axios";

function Artikal(product: productDto) {
  
  return (
    <>
    <div className="card shadow-lg rounded-2xl kartica">
      <Link to={`/product/${product._id}`}>
        <h2 className="card-titel">{product.name}</h2>
        {/* <p className="text-gray-700">
          Brend: <span className="font-semibold">{item.brend}</span>
        </p> */}
        <p className="text-gray-700">
          Grama: <span className="font-semibold">{product.weight_g}g</span>
        </p>
        <p className="text-gray-700">
          Cena: <span className="font-semibold">{product.price} RSD</span>
        </p>
        
        <button
          className={`mt-4 px-4 py-2 rounded-lg text-black "bg-blue-500 hover:bg-blue-600"`}
        >
          Dodaj u korpu
        </button>
      </Link>
    </div>
    </>
  );
}

export default Artikal;
