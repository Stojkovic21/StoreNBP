import { useEffect, useState } from "react";
import { useForm, SubmitHandler } from "react-hook-form";
import "./ProduktPage.css";
import { useParams } from "react-router-dom";
import productDto from "../../DTOs/ProductDto";
import { axiosPrivate } from "../../api/axios";
import Header from "../Header/Header";
import CartDTO from "../../DTOs/CartDTO";
import useShoppingCart from "../../hooks/useShoppingCart";
type quantityType = {
  quantity: number;
};
const ProductCard = () => {
  const param = useParams();
  const [product, setProduct] = useState<productDto>();
  const [kolicina, setKolicina] = useState<number>(0);
  const {setQuantityIsChangedGlobal}=useShoppingCart();
  // const {incCartQuanity,cartQuantity} = useShoppingCart();
  // Inicijalizacija sa definisanim tipom <OrderFormInputs>
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<quantityType>();

  useEffect(() => {
    const fetchItems = async () => {
      const response = await axiosPrivate.get(
        `/product/get/id:${param.productid}`
      );
      setProduct(response.data);
      //console.log(response.data);
    };
    fetchItems();
  },[]);

  // Handler za submit
  const onSubmit: SubmitHandler<quantityType> = async (data) => {
    if(kolicina!==0)
    {
      const cart: CartDTO = {
        Id: product?._id,
        Name: product?.name,
        Price: product?.price,
        Quantity: kolicina,
      };
      
      await axiosPrivate.post("/cart/new", cart);
      setQuantityIsChangedGlobal();
    }else {console.log("Kolicina 0");
    }
    // console.log('TS Porudžbina:', {
    //   productId: product.id,
    //   productName: product.title,
    //   quantity: data.quantity
    // });

    // alert(`Uspešno poručeno: ${data.quantity} kom. artikla "${product.title}"`);
    // reset();
  };

  return (
    <>
      <Header />
      <div className="product-container">
        <div className="product-card">
          
          <div className="product-image-section">
            mesto za sliku
            {/* <img src={product.image} alt={product.name} className="product-img" /> */}
          </div>

          <div className="product-details-section">
            <h1 className="product-title">{product?.name}</h1>

            <p className="product-price">
              {product?.price.toLocaleString()} {"RDS"}
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
                
                  {!kolicina&&<div className="redError">*Broj porudzbina ne moze biti 0</div>}
                
              </div>

              <button type="submit" className="btn-order">
                Dodaj u korpu
              </button>
            </form>
          </div>
        </div>
      </div>
    </>
  );
};

export default ProductCard;
