import { useState } from "react";
import useShoppingCart from "../../hooks/useShoppingCart";
import "./ShopingCart.css";
import axios from "../../api/axios";

type CartItem = {
  id: string;
  name: string;
  price: number;
  quantity: number;
};
export default function ItemCartPresent(product: CartItem) {
  const [onBoardQuantity, setOnBoardQuantity] = useState<number>(
    product.quantity
  );
  const { decCartQuantity, setQuantityIsChangedGlobal } = useShoppingCart();
  async function deleteFromCart() {
    await axios.delete(`/cart/remove/${product.id}`,{withCredentials:true});
    decCartQuantity();
    setQuantityIsChangedGlobal();
  }
  return (
    <>
      <div className="cart-item">
        <div>
          <strong>{product.name}</strong>
          <div className="cart-item">
            <label
              className="minusplus"
              onClick={async () => {
                if (product.quantity > 0) {
                  await axios.patch(`cart/inc/${product.id}/${-1}`,null,{withCredentials:true});
                  setOnBoardQuantity(onBoardQuantity - 1);
                }
              }}
            >
              {"- "}
            </label>
            <label>{onBoardQuantity} </label>
            <label
              className="minusplus"
              onClick={async () => {
                await axios.patch(`cart/inc/${product.id}/${1}`);
                setOnBoardQuantity(onBoardQuantity + 1);
              }}
            >
              {" +"}
            </label>
          </div>
        </div>
        <div className="cart-item">{onBoardQuantity * product.price} din.</div>
        <div
          className="close-button"
          onClick={() => {
            deleteFromCart();
          }}
        >
          x
        </div>
      </div>
    </>
  );
}
