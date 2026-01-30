import axios from "../../api/axios";
import useShoppingCart from "../../hooks/useShoppingCart";
import ItemCartPresent from "./ItemCartPresent";
import SuccessPopup from "./PopupSuccessfullyAdded";
import "./ShopingCart.css";
import { useEffect, useState } from "react";
type ShoppingCartProps = {
  isOpen: boolean;
};
type CartItem = {
  id: string;
  name: string;
  price: number;
  quantity: number;
};
function ShoppingCart({ isOpen }: ShoppingCartProps) {
  const { closeCart, incCartQuantity, quantityIsChanged } = useShoppingCart();
  const [separateItems, setSeparateItem] = useState<CartItem[]>([]);
  const [currItems, setCurrItem] = useState<CartItem[]>([]);
  const [totalValue, setTotalValue] = useState<number>(0);
  const [showPopup, setShowPopup] = useState(false);
  useEffect(() => {
    const itemsInCart = async () => {
      try {
        const results = await axios.get("/cart/get", { withCredentials: true });
        setCurrItem(results.data);
      } catch (error) {}
    };
    itemsInCart();
  }, [quantityIsChanged]);

  useEffect(() => {
    const pomNiz = [];
    let value = 0;
    for (var item in currItems) {
      pomNiz.push(currItems[item]);
      incCartQuantity();
      value += currItems[item].price * currItems[item].quantity;
    }
    setTotalValue(value);
    setSeparateItem(pomNiz);
  }, [currItems]);

  async function checkOut() {
    // kad chackoutuje treba da izadje popup window sa kompletnom porudzbinom i poljem za unos notes-a
    // ovo ako stignem i imam vreme
    const Orders = [];
    for (var item in currItems) {
      Orders.push(currItems[item]);
    }

    await axios
      .post(
        `bill/create`,
        { products: Orders, note: "" },
        { withCredentials: true },
      )
      .then((response) => {
        if (response.status === 200) {
          setShowPopup(true);
          setTimeout(() => setShowPopup(false), 2000);
        }
      });
  }
  return (
    <>
      <SuccessPopup show={showPopup} text="Uspesno dodato u korpu" />
      <div className={`cart-drawer ${isOpen ? "open" : ""}`}>
        <div className="cart-header">
          <h2>Your Cart</h2>
          <button
            className="close-button"
            onClick={() => {
              closeCart();
            }}
          >
            ×
          </button>
        </div>
        <div>
          <div className="cart-items">
            {separateItems.length === 0 ? (
              <p className="empty">Cart is empty.</p>
            ) : (
              separateItems.map((product, i) => (
                <ItemCartPresent key={i} {...product} />
              ))
            )}
          </div>

          <div className="cart-footer">
            <strong>Total:</strong> {totalValue} din.
            <button className="checkout-button" onClick={() => checkOut()}>
              Checkout
            </button>
          </div>
        </div>
      </div>

      {isOpen && <div className="overlay" onClick={() => closeCart()} />}
    </>
  );
}

export default ShoppingCart;
