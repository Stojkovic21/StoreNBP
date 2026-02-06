import { useState } from "react";
import BillDTO from "../../DTOs/BillDTO";
import "./BillStyle.css"

export default function BillRow(bill:BillDTO){
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div className={`bill-container ${isOpen ? 'active' : ''}`}>
      <div className="bill-header" onClick={() => setIsOpen(!isOpen)}>
        <div className="bill-info">
            <span className="bill-date">{bill.date}</span>
            <span className="bill-note">{bill.note}</span>
        </div>
        <div className="bill-total">
          {bill.totalPrice.toFixed(2)} RSD
          <span className={`arrow ${isOpen ? 'up' : 'down'}`}>▼</span>
        </div>
      </div>
      {isOpen && (
        <div className="bill-details">
          <ul className="items-list">
            <li className="item-row">
            <span className="item-name">Name</span>
                <span className="item-name">Price  quantity</span>
                <span className="item-price">Total price</span>
                </li>
                {bill.orders?.map((product) => (
                <li key={product.product._id} className="item-row">
                    <span className="item-name">{product.product.name}</span>
                    <span className="item-name">{product.product.price} x {product.quantity}</span>
                    <span className="item-price">{product.product.price} RSD</span>
                </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
};