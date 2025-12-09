//import { FormEvent, useCallback, useState } from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import "./AddItem.css";
import { useEffect, useState } from "react";
import itemDTo from "../../DTOs/ItemDto";
import "../style/Card.css";
import Header from "../Header/Header";
import RelationshipDTO from "../../DTOs/RelationshipDTO";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import { Link } from "react-router-dom";

function Additem() {
  const [items, setItems] = useState<itemDTo[]>([]);
  const axiosPrivate = useAxiosPrivate();
  useEffect(() => {
    const fetchItems = async () => {
      try {
        const responseItem = await axiosPrivate.get("/item/get/all");
        setItems(responseItem.data.items);
      } catch (err) {
      } finally {
      }
    };
    fetchItems();
  }, []);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<itemDTo>();

  const onSubmit: SubmitHandler<itemDTo> = async (data) => {


    //await axiosPrivate.post("/item/add", data);
    //await new Promise((resolve) => setTimeout(resolve, 1000));
//    await axiosPrivate.post("/relationship/supplier/connect", brendConnection);
    // await axiosPrivate.post(
    //   "/relationship/category/connect",
    //   categoryConnection
    // );
    //window.location.reload();
  };

  return (
    <>
      <Header />
      <Link to={"/item"}>
        <div className="card-container">
          <div className="card">
            <h2>Add New Item</h2>
            <form onSubmit={handleSubmit(onSubmit)}>
              <div className="mb-3">
                <label className="form-label">Name</label>
                <input
                  {...register("name", { required: "*Name is required" })}
                  type="text"
                  className="form-control"
                  name="name"
                  placeholder="Enter item name"
                />
              </div>
              {errors.name && (
                <div className="redError">{errors.name.message}</div>
              )}
              <div className="mb-3">
                <label className="form-label">Price</label>
                <input
                  {...register("price", { required: "*Price is required" })}
                  type="number"
                  className="form-control"
                  name="price"
                  placeholder="Enter item price"
                />
              </div>
              {errors.price && (
                <div className="redError">{errors.price.message}</div>
              )}
              <div className="mb-3">
                <label className="form-label">Weight in g</label>
                <input
                  {...register("weight_g", {
                    required: "*weight is require",
                  })}
                  type="number"
                  className="form-control"
                  name="weight_g"
                  placeholder="Enter neto quantity here"
                />
              </div>
              {errors.weight_g && (
                <div className="redError">{errors.weight_g.message}</div>
              )}
              <button
                disabled={isSubmitting}
                type="submit"
                className="btn btn-primary"
              >
                {isSubmitting ? "Loading..." : "Submit"}
              </button>
            </form>
          </div>
        </div>
      </Link>
    </>
  );
}

export default Additem;
