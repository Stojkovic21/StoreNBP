import productDto from "../../DTOs/ProductDto";
import Artikal from "../Artikal/Artikal";
import "./Polica.css";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import { useEffect, useState } from "react";

function Polica() {
  const [items, setItems] = useState<productDto[]>([]);
  const axiosPrivate = useAxiosPrivate();
  useEffect(() => {
    const fetchItems = async () => {
      try {
        const response = await axiosPrivate.get("/product/get/all");
        setItems(response.data);
      } catch (err) {
      } finally {
      }
    };

    fetchItems();
  }, []);

  return (
    <>
      <div className="polica">
        {items
          ? items.map((item) => <Artikal key={item._id} {...item} />)
          : null}
      </div>
    </>
  );
}

export default Polica;
