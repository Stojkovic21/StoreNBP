import productDto from "./ProductDto";

interface BillDTO {
  id: number;
  totalPrice: number;
  date: string;
  note: string;
  orders: OrdersDTO[];
}
interface OrdersDTO{
  quantity:number;
  product:productDto
}
export default BillDTO