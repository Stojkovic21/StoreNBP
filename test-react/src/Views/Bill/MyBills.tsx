import { useEffect, useState } from "react";
import Header from "../Header/Header";
import axios from "../../api/axios";
import BillRow from "./BillRaw";
import BillDTO from "../../DTOs/BillDTO";
import useAuth from "../../hooks/useAuth";

export default function MyBills(){
    const {customerId}=useAuth();
    const[bills,setBills]=useState<BillDTO[]>([]);
    useEffect(()=>{
        const fetchItems = async () => {
            try {
                const response = await axios.get(`/bill/get/customer/${customerId}`);
                setBills(response.data);
                } catch (err) {
                }
        };
        fetchItems();
    },[]);
  return (<>
        <Header/>
        <div className="page-wrapper">
        <h1>My bills</h1>
        <div className="bills-list">
            {bills.map((bill) => (
            <BillRow key={bill.id} {...bill} />
            ))}
        </div>
        </div>
    </>
  );
};
