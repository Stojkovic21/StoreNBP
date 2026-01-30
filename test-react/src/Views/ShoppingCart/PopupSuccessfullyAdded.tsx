import { useEffect, useState } from "react";
import "./Popup.css";
type popupType={
  show:boolean
  text:string
}
export default function SuccessPopup({show,text}:popupType) {
  //if (!show) return null;
const [visible, setVisible] = useState(show);

  useEffect(() => {
    if (show) {
      setVisible(true);
    } else {
      const timer = setTimeout(() => setVisible(false), 200);
      return () => clearTimeout(timer);
    }
  }, [show]);
  return (
    visible&&
      <div className={`popup ${show ? "enter" : "exit"}`}>
        <h3>Uspešno!</h3>
        <p>{text} 😄</p>
      </div>
  );
}
