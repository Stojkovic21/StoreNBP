import { useEffect, useState } from "react";
import { useForm, SubmitHandler } from "react-hook-form";
import "./ProduktPage.css";
import { useNavigate, useParams } from "react-router-dom";
import productDto from "../../DTOs/ProductDto";
import axios, { axiosPrivate } from "../../api/axios";
import Header from "../Header/Header";
import CartDTO from "../../DTOs/CartDTO";
import useShoppingCart from "../../hooks/useShoppingCart";
import SuccessPopup from "../ShoppingCart/PopupSuccessfullyAdded";
import Recommendation from "../Recommendation/Recommendation";
type quantityType = {
  quantity: number;
};
const ProductCard = () => {
  const param = useParams();
  const navigate=useNavigate();
  const [product, setProduct] = useState<productDto>();
  const [kolicina, setKolicina] = useState<number>(1);
  const { setQuantityIsChangedGlobal} = useShoppingCart();
  const { handleSubmit, formState:{isSubmitting}} = useForm<quantityType>();
  const [showPopup, setShowPopup]=useState(false);

  useEffect(() => {
    const fetchItems = async () => {
      const response = await axiosPrivate.get(
        `/product/get/id:${param.productid}`,
      );
      setProduct(response.data);
    };
    fetchItems();
  }, []);

  const onSubmit: SubmitHandler<quantityType> = async () => {
    if (kolicina !== 0) {
      const cart: CartDTO = {
        Id: product?._id,
        Name: product?.name,
        Price: product?.price,
        Quantity: kolicina,
      };

      await axiosPrivate.post("/cart/new", cart).then((response) => {
        if (response.status === 200) {
          setShowPopup(true);
          setTimeout(() => setShowPopup(false), 2000);
        }
      });
      setQuantityIsChangedGlobal();
    } else {
      console.log("Kolicina 0");
    }
  };
  const Edit=()=>{
    navigate(`/product/edit/${param.productid}`);
  };
  const Delete=async()=>{
    try {
      await axios.delete(`/product/delete/${param.productid}`);
      navigate("/");
    } catch (error) {
      
    }
  };

  return (
    <>
      <Header />
      <SuccessPopup
        show={showPopup} text="Uspesno dodato u korpu"
      />
      <div className="product-container">
        <div className="product-card">
          <div className="product-image-section">
             <img src={`/src/Images/${product?.image}`} alt={product?.name} className="product-img" />
          </div>

          <div className="product-details-section">
            <h1 className="product-title">{product?.name}</h1>

            <p className="product-price">
              {product?.price.toLocaleString()} {"RSD"}
            </p>

            <p className="product-description">{product?.description}</p>

            <form onSubmit={handleSubmit(onSubmit)} className="order-form">
              <div className="form-group">
                <label htmlFor="quantity">Količina:</label>
                <div className="kolicina">
                  <label
                    onClick={() =>
                      setKolicina(kolicina >= 1 ? kolicina - 1 : 0)
                    }
                  >
                    {" "}
                    {"-"}{" "}
                  </label>
                  <label>{kolicina}</label>
                  <label onClick={() => setKolicina(kolicina + 1)}>
                    {" "}
                    {"+"}{" "}
                  </label>
                </div>

                {!kolicina && (
                  <div className="redError">
                    *Broj porudzbina ne moze biti 0
                  </div>
                )}
              </div>

              <button type="submit" className="btn-order" disabled={isSubmitting}>
                Dodaj u korpu
              </button>
            </form>
            <div className="button-wrapper">
              <button className="btn btn-edit" onClick={Edit}>Edit</button>
              <button className="btn btn-delete" onClick={Delete}>Delete</button>
            </div>
          </div>
        </div>
        <Recommendation/>
      </div>
    </>
  );
};

export default ProductCard;
