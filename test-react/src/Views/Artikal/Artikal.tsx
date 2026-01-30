import { Link } from "react-router-dom";
import productDto from "../../DTOs/ProductDto";
import "./Artikal.css";

function Artikal(product: productDto) {
  
  return (
    <>
    <div className="product-cardd">
  <Link to={`/product/${product._id}`} className="card-link">
    
    {/* Slika Proizvoda */}
    <div className="card-image-container">
      <img 
        //src={product.image || "https://via.placeholder.com/300"} 
        alt={product.name+" slika"} 
        className="card-image" 
      />
    </div>

    {/* Sadržaj */}
    <div className="card-content">
      <h2 className="card-title">{product.name}</h2>
      
      <p className="card-price">
        {product.price} RSD
      </p>
    </div>

  </Link>
</div>
    </>
  );
}

export default Artikal;
