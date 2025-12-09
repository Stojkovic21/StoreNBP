import { useEffect, useState } from 'react';
import { useForm, SubmitHandler } from 'react-hook-form';
import './ProduktPage.css';
import { useParams } from 'react-router-dom';
import itemDto from '../../DTOs/ItemDto';
import { axiosPrivate } from '../../api/axios';
import Header from '../Header/Header';
type quantityType=
{
  quantity:number
}
const ProductCard = () => {
  const param=useParams();
  const [product, setProduct]=useState<itemDto>()
  // Inicijalizacija sa definisanim tipom <OrderFormInputs>
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<quantityType>();

  useEffect(()=>{
    const fetchItems=async () => {
      const response= await axiosPrivate.get(`/item/get/id:${param.productid}`);
      setProduct(response.data);
      console.log(response.data);
    }
    fetchItems();
  });

  // Handler za submit
  const onSubmit: SubmitHandler<quantityType> = (data) => {
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
      <Header/>
      <div className="product-container">
        <div className="product-card">
          
          {/* LEVA STRANA - SLIKA */}
          <div className="product-image-section">
            mesto za sliku{/* <img src={product.image} alt={product.name} className="product-img" /> */}
          </div>

          {/* DESNA STRANA - DETALJI */}
          <div className="product-details-section">
            <h1 className="product-title">{product?.name}</h1>
            
            <p className="product-price">
              {product?.price.toLocaleString()} {"RDS"}
            </p>

            <p className="product-description">
              {"product?.description"}
            </p>

            {/* FORMA ZA PORUČIVANJE */}
            <form onSubmit={handleSubmit(onSubmit)} className="order-form">
              <div className="form-group">
                <label htmlFor="quantity">Količina:</label>
                <input
                  id="quantity"
                  type="number"
                  defaultValue={20}
                  {...register("quantity", { 
                    required: "Količina je obavezna", 
                    min: { value: 20, message: "Minimum je 20 komad" },
                    // max: { value: 10, message: "Maksimum je 10 komada" }
                  })}
                />
                {errors.quantity && <span className="error-msg">{errors.quantity.message}</span>}
              </div>

              <button type="submit" className="btn-order">
                Poruči Proizvod
              </button>
            </form>
          </div>
        </div>
      </div>
    </>
  );
};

export default ProductCard;