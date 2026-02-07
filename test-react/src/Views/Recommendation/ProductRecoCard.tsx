import { useNavigate } from "react-router-dom";
import productDto from "../../DTOs/ProductDto";
import "./Recommendation.css";
export default function ProductRecoCard(product: productDto) {
  const navigate = useNavigate();
  return (
    <div
      className="product-reco-card"
      onClick={() => {
        navigate(`/product/${product._id}`);
        window.location.reload();
      }}
    >
      <div className="product-reco-image-wrapper">
        <img src={`/src/Images/${product.image}`} alt={product.name} className="product-reco-image" />
      </div>
      <div className="product-reco-info">
        <h3 className="product-reco-name">{product.name}</h3>
        <p className="product-reco-price">{product.price} rsd</p>
      </div>
    </div>
  );
}
