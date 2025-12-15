import axios from "../../api/axios";
import useShoppingCart from "../../hooks/useShoppingCart";
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
   const { closeCart, incCartQuantity,decCartQuantity,quantityIsChanged,setQuantityIsChangedGlobal} =useShoppingCart();

  const [separateItems, setSeparateItem] = useState<CartItem[]>([]);
  const [currItems, setCurrItem] = useState<CartItem[]>([]);
  const [onBoardQuantity,setOnBoardQuantity]=useState<number>(0);
  useEffect(() => {
    const itemsInCart = async () => {
      try {
        const results = await axios.get("/cart/get");
        setCurrItem(results.data);
      } catch (error) {}
    };
    itemsInCart();
    
  },[quantityIsChanged]);

  useEffect(() => {
    const pomNiz = [];
    for (var item in currItems) {
      pomNiz.push(currItems[item]);
      incCartQuantity();
      
    }
    setSeparateItem(pomNiz);
  }, [currItems]);

  async function deleteFromCart(id: string)
  {
    await axios.delete(`/cart/remove/${id}`);
    decCartQuantity();
    setQuantityIsChangedGlobal();
  }

  return (
    <>
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
        <form>
          <div className="cart-items">
            {separateItems.length === 0 ? (
              <p className="empty">Cart is empty.</p>
            ) : (
              separateItems.map((product, i) => (
                <div key={i} className="cart-item">
                  <div>
                    <strong>{product.name}</strong>
                    <div className="cart-item">
                      <label className="minusplus" onClick={async ()=>{await axios.patch(`cart/inc/${product.id}/${-1}`);setOnBoardQuantity(onBoardQuantity-1);product.quantity--;
                      }}>
                        {"- "}
                      </label>
                      <label onMouseEnter={()=>{setOnBoardQuantity(product.quantity);}}>
                        {onBoardQuantity} {/*onBoardQuantity kao improvizacija za vizualni priklaz trenutne kolicine porucenig produkta */}
                      </label>
                      <label className="minusplus" onClick={async ()=>{await axios.patch(`cart/inc/${product.id}/${1}`);setOnBoardQuantity(onBoardQuantity+1);product.quantity++
                      }}>
                        {' +'}
                      </label>
                    </div>
                  </div>
                  <div className="cart-item">
                    {product.quantity * product.price} din.
                  </div>
                  <div className="close-button" onClick={()=>{deleteFromCart(product.id);
                  }}>
                    x
                  </div>
                </div>
              ))
            )}
          </div>

          <div className="cart-footer">
            <strong>Total:</strong> {1312} din.
            <button className="checkout-button" onClick={() => console.log("")}>
              Checkout
            </button>
          </div>
        </form>
      </div>

      {isOpen && <div className="overlay" onClick={() => closeCart()} />}
    </>
  );
}

export default ShoppingCart;
