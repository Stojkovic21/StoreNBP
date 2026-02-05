import { createContext, PropsWithChildren, useState } from "react";
import ShoppingCart from "../Views/ShoppingCart/ShoppingCart";

const ShoppingCartContext = createContext<
  ShoppingCartProviderValue | undefined
>(undefined);

type ShoppingCartProps = PropsWithChildren;

export type ShoppingCartProviderValue = {
  openCart: () => void;
  closeCart: () => void;
  cartQuantity: number;
  incCartQuantity: ()=>void;
  decCartQuantity: ()=>void;
  quantityIsChanged:boolean;
  setQuantityIsChangedGlobal:()=>void;
};
export function ShoppingCardProvider({ children }: ShoppingCartProps) {
  let [cartQuantity, setCartQuantity] = useState<number>(0);
  const [isOpen, setIsOpen] = useState(false);
  const [quantityIsChanged, setQuantityIsChanged]=useState(false);

  function setQuantityIsChangedGlobal(){
    if(quantityIsChanged==false) setQuantityIsChanged(true);
    else setQuantityIsChanged(false);
  }
  function incCartQuantity(){setCartQuantity(cartQuantity++);
  }
  function decCartQuantity(){setCartQuantity(cartQuantity--);}
  function openCart() {
    setIsOpen(true);
  }
  function closeCart() {
    setIsOpen(false);
  }
  return (
    <ShoppingCartContext
      value={{
        openCart,
        closeCart,
        cartQuantity,
        incCartQuantity,
        decCartQuantity,
        quantityIsChanged,
        setQuantityIsChangedGlobal,
      }}
    >
      {children}
      <ShoppingCart isOpen={isOpen} />
    </ShoppingCartContext>
  );
}

export default ShoppingCartContext;
