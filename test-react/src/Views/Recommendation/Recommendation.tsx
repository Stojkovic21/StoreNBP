import { useEffect, useState } from "react";
import productDto from "../../DTOs/ProductDto";
import axios from "../../api/axios";
import ProductRecoCard from "./ProductRecoCard";
import { useParams } from "react-router-dom";
import "./Recommendation.css";

export default function Recommendation()
{
    const param=useParams();
    const [products, setProducts] = useState<productDto[]>([]);
      useEffect(() => {
        const fetchItems = async () => {
          try {
            const response = await axios.get(`/recommendation/alsobuy/${param.productid}`);
            setProducts(response.data);
          } catch (err) {
          } finally {
          }
        };
    
        fetchItems();
      }, []);
    
      return (
    <div className="product-reco-section">
      <h3 className="product-reco-title">Preporuke</h3>
      <div className="product-reco-list-container">
        {products.map((product) => (
          <ProductRecoCard key={product._id} {...product} />
        ))}
      </div>
    </div>
  );
}