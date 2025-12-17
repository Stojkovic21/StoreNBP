//import { FormEvent, useCallback, useState } from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import "./AddProduct.css";
import { useState } from "react";
import productDTo from "../../DTOs/ProductDto";
import "../style/Card.css";
import Header from "../Header/Header";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";

function AddProduct() {
  const axiosPrivate = useAxiosPrivate();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<productDTo>();

  const [text, setText] = useState("");
  const [isOpen, setIsOpen] = useState(false);

  const onSubmit: SubmitHandler<productDTo> = async (data) => {
    await axiosPrivate.post("/product/add", data);
    //window.location.reload();
  };

  return (
    <>
      <Header />
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

            <div className="input-wrapper">
              <label className="form-label">Description</label>

              <input
                type="text"
                className="trigger-input"
                value={text}
                onClick={() => setIsOpen(true)}
                placeholder="Enter description"
                readOnly
              />

              {isOpen && (
                <div className="modal-overlay" onClick={() => setIsOpen(false)}>
                  
                  <div
                    className="modal-content"
                    onClick={(e) => e.stopPropagation()}
                  >
                    <h2 className="modal-title">Product description</h2>

                    <textarea
                    {...register("description",{required:"*Description is required"})}
                      className="modal-textarea"
                      value={text}
                      onChange={(e) => setText(e.target.value)}
                      placeholder="Description..."
                      autoFocus
                    />
                    {errors.description&&(
                      <div className="redError">{errors.description.message}</div>
                    )}

                    <button
                      className="modal-save-btn"
                      onClick={() => setIsOpen(false)}
                    >
                      Save and close
                    </button>
                  </div>
                </div>
              )}
            </div>
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
    </>
  );
}

export default AddProduct;
