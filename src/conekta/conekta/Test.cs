﻿using NUnit.Framework;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using conekta;

namespace ConektaTest
{
	[TestFixture()]
	public class ListTest
	{

		[Test()]
		public void getObject()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			conekta.Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
				   ""name"": ""Box of Cohiba S1s"",
				   ""unit_price"": 35000,
				   ""quantity"": 1
				}]
	        }");

			order = new Order().find(order.id);

			LineItem line_item = (LineItem)order.line_items.at(0);

			Assert.AreEqual(line_item.unit_price, 35000);
		}
	}

	[TestFixture()]
	public class OrderTest
	{

		private int RandomNumber(int min, int max, int seed = 0)
		{
			Random random = new Random((int)DateTime.Now.Ticks + seed);
			return random.Next(min, max);
		}

		[Test()]
		public void apiVersionUnsupported()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "1.0.0";

			try
			{
				new conekta.Order().create(@"{
		            ""currency"":""MXN"",
					""customer_info"": {
						""name"": ""Jul Ceballos"",
						""phone"": ""+5215555555555"",
						""email"": ""jul@conekta.io""
					}
	        	}");
			}
			catch (ConektaException e)
			{
				Assert.AreEqual(e._object, "error");
				Assert.AreEqual(e._type, "api_version_unsupported");
			}
		}

		[Test()]
		public void createCard()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
	              ""name"": ""Box of Cohiba S1s"",
	              ""unit_price"": 35000,
	              ""quantity"": 1
	            }],
				""charges"": [{
					""payment_source"": {
						""type"": ""card"",
						""token_id"": ""tok_test_visa_4242""
					}
				}]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "fulfilled");
			Assert.AreEqual(order.amount, 35000);

			order = new Order().find(order.id);

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "fulfilled");
			Assert.AreEqual(order.amount, 35000);

			order = order.createReturn(@"{""amount"": 35000}");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "returned");
			Assert.AreEqual(order.amount, 35000);

			Order[] orders = new Order().where(new JObject());
			Assert.AreEqual(orders[0].id.GetType().ToString(), "System.String");
		}

		[Test()]
		public void createCharge()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
	              ""name"": ""Box of Cohiba S1s"",
	              ""unit_price"": 35000,
	              ""quantity"": 1
	            }]
	        }");

			order.createCharge(@"{
				""payment_source"": {
					""type"": ""card"",
					""token_id"": ""tok_test_visa_4242""
				},
				""amount"": 35000
			}");

			order = new Order().find(order.id);

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "fulfilled");
			Assert.AreEqual(order.amount, 35000);

			order = new Order().find(order.id);

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "fulfilled");
			Assert.AreEqual(order.amount, 35000);

			order = order.createReturn(@"{""amount"": 35000}");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "returned");
			Assert.AreEqual(order.amount, 35000);

			Order[] orders = new Order().where(new JObject());
			Assert.AreEqual(orders[0].id.GetType().ToString(), "System.String");
		}

		[Test()]
		public void captureCharge()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
	              ""name"": ""Box of Cohiba S1s"",
	              ""unit_price"": 35000,
	              ""quantity"": 1
	            }],
				""preauthorize"": true
	        }");
			
			order.createCharge(@"{
		        ""payment_source"": {
		            ""type"": ""card"",
					""token_id"": ""tok_test_visa_4242""
		        },
		        ""amount"": 35000
			}");

			order = new Order().find(order.id);

			order = order.capture();

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "fulfilled");
			Assert.AreEqual(order.amount, 35000);

			order = new Order().find(order.id);

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "fulfilled");
			Assert.AreEqual(order.amount, 35000);

			order = order.createReturn(@"{""amount"": 35000}");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "returned");
			Assert.AreEqual(order.amount, 35000);

			Order[] orders = new Order().where(new JObject());
			Assert.AreEqual(orders[0].id.GetType().ToString(), "System.String");
		}

		[Test()]
		public void createCardError()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			try
			{
				new conekta.Order().create(@"{
		            ""currency"":""MXN"",
					""customer_info"": {
						""name"": ""Jul Ceballos"",
						""phone"": ""+5215555555555"",
						""email"": ""jul@conekta.io""
					}
	        	}");
			}
			catch (ConektaException e)
			{
				Assert.AreEqual(e._object, "error");
				Assert.AreEqual(e._type, "parameter_validation_error");
			}
		}

		[Test()]
		public void CreateOxxo()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
	              ""name"": ""Box of Cohiba S1s"",
	              ""unit_price"": 35000,
	              ""quantity"": 1
	            }],
				""charges"": [{
					""payment_source"": {
						""type"": ""oxxo_cash"",
						""expires_at"": 1513036800
					}
				}]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");
			Assert.AreEqual(order.amount, 35000);

			order = new Order().find(order.id);

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");
			Assert.AreEqual(order.amount, 35000);

			Order[] orders = new Order().where(new JObject());
			Assert.AreEqual(orders[0].id.GetType().ToString(), "System.String");
		}

		[Test()]
		public void update()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
	              ""name"": ""Box of Cohiba S1s"",
	              ""unit_price"": 35000,
	              ""quantity"": 1
	            }]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");
			Assert.AreEqual(order.amount, 35000);

			order = new Order().find(order.id);

			order = order.update(@"{""currency"": ""USD""}");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");
			Assert.AreEqual(order.amount, 35000);

			Order[] orders = new Order().where(new JObject());
			Assert.AreEqual(orders[0].id.GetType().ToString(), "System.String");
		}

		[Test()]
		public void createLineItem()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
				   ""name"": ""Box of Cohiba S1s"",
				   ""unit_price"": 35000,
				   ""quantity"": 1
				}]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");

			order = new Order().find(order.id);

			LineItem line_item = order.createLineItem(@"{
			   ""name"": ""Box of Cohiba S1s"",
			   ""unit_price"": 35000,
			   ""quantity"": 1
			}");

			Assert.AreEqual(line_item.name, "Box of Cohiba S1s");
		}

		[Test()]
		public void updateLineItem()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
				   ""name"": ""Box of Cohiba S1s"",
				   ""unit_price"": 35000,
				   ""quantity"": 1
				}]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");

			order = new Order().find(order.id);

			order.line_items.at(0);

			// Assert.AreEqual(line_item.description, "Imported From Mex.");
		}

		[Test()]
		public void createTaxLine()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
				   ""name"": ""Box of Cohiba S1s"",
				   ""unit_price"": 35000,
				   ""quantity"": 1
				}]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");

			order = new Order().find(order.id);

			TaxLine tax_line = order.createTaxLine(@"{
			   ""description"": ""IVA"",
			   ""amount"": 600,
			   ""metadata"": {
			     ""random_key"": ""random_value""
			   }
			}");
	
			Assert.AreEqual(tax_line.description, "IVA");
			Assert.AreEqual(tax_line.amount, 600);
		}

		[Test()]
		public void updateTaxLine()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
				   ""name"": ""Box of Cohiba S1s"",
				   ""unit_price"": 35000,
				   ""quantity"": 1,
				   ""metadata"": {
				      ""random_key"": ""random value""
				   }
				}]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");

			order = new Order().find(order.id);

			TaxLine tax_line = order.createTaxLine(@"{
			   ""description"": ""IVA"",
			   ""amount"": 600,
			   ""contextual_data"": {
			     ""random_key"": ""random_value""
			   }
			}");

			order = new Order().find(order.id);

			//tax_line = order.tax_lines.at(0).update(@"{
			//   ""description"": ""IVA"",
			//   ""amount"": 1000,
			//   ""contextual_data"": {
			//      ""random_key"": ""random_value""
			//   }
			//}");

			//System.Console.WriteLine(tax_line.amount);

			//Assert.AreEqual(tax_line.description, "IVA");
			//Assert.AreEqual(tax_line.amount, 1000);
		}

		[Test()]
		public void createShippingLine()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
				   ""name"": ""Box of Cohiba S1s"",
				   ""unit_price"": 35000,
				   ""quantity"": 1
				}]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");

			order = new Order().find(order.id);

			ShippingLine shipping_line = order.createShippingLine(@"{
			    ""amount"": 0,
			    ""tracking_number"": ""TRACK123"",
			    ""carrier"": ""USPS"",
			    ""method"": ""Train"",
			    ""metadata"": {
			       ""random_key"": ""random_value""
			    }
			}");

			Assert.AreEqual(shipping_line.tracking_number, "TRACK123");
		}

		[Test()]
		public void updateShippingLine()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
				   ""name"": ""Box of Cohiba S1s"",
				   ""unit_price"": 35000,
				   ""quantity"": 1
				}]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");

			order = new Order().find(order.id);

			ShippingLine shipping_line = order.createShippingLine(@"{
			    ""amount"": 0,
			    ""tracking_number"": ""TRACK123"",
			    ""carrier"": ""Fedex"",
			    ""method"": ""Train"",
			    ""contextual_data"": {
			       ""random_key"": ""random_value""
			    }
			}");

			order = new Order().find(order.id);

			//shipping_line = order.shipping_lines.at(0).update(@"{
			//   ""description"": ""Free Shipping"",
			//   ""amount"": 0,
			//   ""tracking_number"": ""TRACK456"",
			//   ""carrier"": ""USPS"",
			//   ""method"": ""Train"",
			//   ""contextual_data"": {
			//      ""random_key"": ""random_value""
			//   }
			//}");

			Assert.AreEqual(shipping_line.carrier, "Fedex");
		}

		[Test()]
		public void createDiscountLine()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
				   ""name"": ""Box of Cohiba S1s"",
				   ""unit_price"": 35000,
				   ""quantity"": 1
				}]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");

			order = new Order().find(order.id);

			DiscountLine discount_line = order.createDiscountLine(@"{
			    ""code"": ""123"",
			    ""type"": ""loyalty"",
			    ""amount"": 600
			}");

			Assert.AreEqual(discount_line.code, "123");
			Assert.AreEqual(discount_line.type, "loyalty");
		}

		[Test()]
		public void updateDiscountLine()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Order order = new conekta.Order().create(@"{
	            ""currency"":""MXN"",
				""customer_info"": {
					""name"": ""Jul Ceballos"",
					""phone"": ""+5215555555555"",
					""email"": ""jul@conekta.io""
				},
	            ""line_items"": [{
				   ""name"": ""Box of Cohiba S1s"",
				   ""unit_price"": 35000,
				   ""quantity"": 1
				}]
	        }");

			Assert.AreEqual(order.id.GetType().ToString(), "System.String");
			Assert.AreEqual(order.status, "created");

			order = new Order().find(order.id);

			DiscountLine discount_line = order.createDiscountLine(@"{
			    ""code"": ""234"",
			    ""type"": ""loyalty"",
			    ""amount"": 600
			}");

			order = new Order().find(order.id);

			//discount_line = order.discount_lines[0].update(@"{
			//    ""amount"": 700
			//}");

			Assert.AreEqual(discount_line.amount, 600);
		}
	}

	[TestFixture()]
	public class CustomerTest
	{
		[Test()]
		public void createCustomer()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com"",
			    ""plan_id"": ""gold-plan"",
			    ""corporate"": true,
			    ""payment_sources"": [{
			        ""token_id"": ""tok_test_visa_4242"",
			        ""type"": ""card""
			    }],
			    ""fiscal_entities"": [{
			        ""tax_id"": ""AMGH851205MN1"",
			        ""company_name"": ""Nike SA de CV"",
			        ""email"": ""contacto@nike.mx"",
			        ""phone"": ""+5215544443333"",
			        ""address"": {
			            ""street1"": ""250 Alexis St"",
			            ""street2"": ""fake street"",
			            ""external_number"": ""91"",
			            ""city"": ""Red Deer"",
			            ""state"": ""Alberta"",
			            ""country"": ""CA"",
			            ""postal_code"": ""T4N 0B8""
			        }
			    }],
			    ""shipping_contacts"": [{
			        ""phone"": ""+5215555555555"",
			        ""receiver"": ""Marvin Fuller"",
			        ""between_streets"": ""Ackerman Crescent"",
			        ""address"": {
			            ""street1"": ""250 Alexis St"",
			            ""street2"": ""fake street"",
			            ""street3"": ""fake street"",
			            ""city"": ""Red Deer"",
			            ""state"": ""Alberta"",
			            ""country"": ""CA"",
			            ""postal_code"": ""T4N 0B8"",
			            ""residential"": true
			        }

			    }],
			    ""account_age"": 300,
			    ""paid_transactions"": 5

			}");

			Assert.AreEqual(customer.corporate, true);

			customer = new Customer().find(customer.id);

			Assert.AreEqual(customer.corporate, true);
		}

		[Test()]
		public void updateCustomer()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com"",
			    ""corporate"": true
			}");

			Assert.AreEqual(customer.corporate, true);
			Assert.AreEqual(customer.name, "Emiliano Cabrera");

			customer = new Customer().find(customer.id);

			Assert.AreEqual(customer.corporate, true);
			Assert.AreEqual(customer.name, "Emiliano Cabrera");

			customer = customer.update(@"{
				""corporate"": false,
				""name"": ""Juan Perez""
			}");

			System.Console.WriteLine(customer.name);

			Assert.AreEqual(customer.name, "Juan Perez");
		}

		[Test()]
		public void deleteCustomer()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com"",
			    ""plan_id"": ""gold-plan"",
			    ""corporate"": true,
			    ""payment_sources"": [{
			        ""token_id"": ""tok_test_visa_4242"",
			        ""type"": ""card""
			    }]
			}");

			Assert.AreEqual(customer.corporate, true);

			customer = new Customer().find(customer.id);

			Assert.AreEqual(customer.corporate, true);

			customer = customer.destroy();

			Assert.AreEqual(customer.corporate, true);
		}

		[Test()]
		public void createPaymentSource()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com"",
			    ""plan_id"": ""gold-plan"",
			    ""corporate"": true,
			    ""payment_sources"": [{
			        ""token_id"": ""tok_test_visa_4242"",
			        ""type"": ""card""
			    }]
			}");

			PaymentSource payment_source = customer.createPaymentSource(@"{
			    ""type"": ""card"",
			    ""name"": ""Emiliano Cabrera"",
			    ""number"": ""4242424242424242"",
			    ""exp_month"": ""12"",
			    ""exp_year"": ""20"",
			    ""cvc"": ""123"",
			    ""address"": {
			        ""street1"": ""Tamesis"",
			        ""street2"": ""114"",
			        ""city"": ""Monterrey"",
			        ""state"": ""Nuevo Leon"",
			        ""country"": ""MX"",
			        ""postal_code"": ""64700""
			    }
			}");

			Assert.AreEqual(payment_source.type, "card");
			Assert.AreEqual(payment_source.name, "Emiliano Cabrera");
		}

		[Test()]
		public void updatePaymentSource()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com"",
			    ""corporate"": true
			}");

			PaymentSource payment_source = customer.createPaymentSource(@"{
			    ""type"": ""card"",
			    ""name"": ""Emiliano Cabrera"",
			    ""number"": ""4242424242424242"",
			    ""exp_month"": ""12"",
			    ""exp_year"": ""20"",
			    ""cvc"": ""123"",
			    ""address"": {
			        ""street1"": ""Tamesis"",
			        ""street2"": ""114"",
			        ""city"": ""Monterrey"",
			        ""state"": ""Nuevo Leon"",
			        ""country"": ""MX"",
			        ""postal_code"": ""64700""
			    }
			}");

			payment_source = payment_source.update(@"{
				""name"": ""Emiliano Suarez""
			}");

			Assert.AreEqual(payment_source.name, "Emiliano Suarez");
		}

		[Test()]
		public void deletePaymentSource()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com""
			}");

			PaymentSource payment_source = customer.createPaymentSource(@"{
			    ""type"": ""card"",
			    ""name"": ""Emiliano Cabrera"",
			    ""number"": ""4242424242424242"",
			    ""exp_month"": ""12"",
			    ""exp_year"": ""20"",
			    ""cvc"": ""123"",
			    ""address"": {
			        ""street1"": ""Tamesis"",
			        ""street2"": ""114"",
			        ""city"": ""Monterrey"",
			        ""state"": ""Nuevo Leon"",
			        ""country"": ""MX"",
			        ""postal_code"": ""64700""
			    }
			}");

			payment_source = payment_source.update(@"{
				""name"": ""Emiliano Suarez""
			}");

			Assert.AreEqual(payment_source.name, "Emiliano Suarez");

			payment_source = payment_source.destroy();

			Assert.AreEqual(payment_source.name, "Emiliano Suarez");
		}

		[Test()]
		public void createShippingContact()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com"",
			    ""plan_id"": ""gold-plan"",
			    ""corporate"": true,
			    ""payment_sources"": [{
			        ""token_id"": ""tok_test_visa_4242"",
			        ""type"": ""card""
			    }],
			    ""shipping_contacts"": [{
			        ""email"": ""thomas.logan@xmen.org"",
			        ""phone"": ""+5215555555555"",
			        ""receiver"": ""Marvin Fuller"",
			        ""between_streets"": ""Ackerman Crescent"",
			        ""address"": {
			            ""street1"": ""250 Alexis St"",
			            ""street2"": ""fake street"",
			            ""city"": ""Red Deer"",
			            ""state"": ""Alberta"",
			            ""country"": ""CA"",
			            ""postal_code"": ""T4N 0B8"",
			            ""residential"": true
			        }
			    }],
			    ""account_age"": 300,
			    ""paid_transactions"": 5
			}");

			ShippingContact shipping_contact = customer.createShippingContact(@"{
			    ""email"": ""thomas.logan@xmen.org"",
			    ""phone"": ""+5215555555555"",
			    ""receiver"": ""Marvin Fuller"",
			    ""between_streets"": ""Ackerman Crescent"",
			    ""address"": {
			        ""street1"": ""250 Alexis St"",
			        ""street2"": ""fake street"",
			        ""city"": ""Red Deer"",
			        ""state"": ""Alberta"",
			        ""country"": ""CA"",
			        ""postal_code"": ""T4N 0B8"",
			        ""residential"": true
			    }
			}");

			Assert.AreEqual(shipping_contact.phone, "+5215555555555");
		}

		[Test()]
		public void updateShippingContact()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""5555555555"",
			    ""email"": ""user@example.com"",
			    ""corporate"": true
			}");

			ShippingContact shipping_contact = customer.createShippingContact(@"{
			    ""phone"": ""5555555555"",
			    ""receiver"": ""Marvin Fuller"",
			    ""between_streets"": ""Ackerman Crescent"",
			    ""address"": {
			        ""street1"": ""250 Alexis St"",
			        ""street2"": ""fake street"",
			        ""city"": ""Red Deer"",
			        ""state"": ""Alberta"",
			        ""country"": ""CA"",
			        ""postal_code"": ""T4N 0B8"",
			        ""residential"": true
			    }
			}");

			shipping_contact = shipping_contact.update(@"{
				""phone"": ""6666666666"",
			}");

			Assert.AreEqual(shipping_contact.phone, "6666666666");
		}

		[Test()]
		public void deleteShippingContact()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com"",
			    ""corporate"": true
			}");

			ShippingContact shipping_contact = customer.createShippingContact(@"{
			    ""phone"": ""+5215555555555"",
			    ""receiver"": ""Marvin Fuller"",
			    ""between_streets"": ""Ackerman Crescent"",
			    ""address"": {
			        ""street1"": ""250 Alexis St"",
			        ""street2"": ""fake street"",
			        ""city"": ""Red Deer"",
			        ""state"": ""Alberta"",
			        ""country"": ""CA"",
			        ""postal_code"": ""T4N 0B8"",
			        ""residential"": true
			    }
			}");

			Assert.AreEqual(shipping_contact.phone, "+5215555555555");

			shipping_contact = shipping_contact.update(@"{
				""phone"": ""+5215555555555""
			}");

			Assert.AreEqual(shipping_contact.phone, "+5215555555555");

			shipping_contact = shipping_contact.destroy();

			Assert.AreEqual(shipping_contact.phone, "+5215555555555");
		}

		[Test()]
		public void createFiscalEntity()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com"",
			    ""plan_id"": ""gold-plan"",
			    ""corporate"": true,
			    ""payment_sources"": [{
			        ""token_id"": ""tok_test_visa_4242"",
			        ""type"": ""card""
			    }],
			    ""fiscal_entities"": [{
			        ""tax_id"": ""AMGH851205MN1"",
			        ""company_name"": ""Nike SA de CV"",
			        ""email"": ""contacto@nike.mx"",
			        ""phone"": ""+5215544443333"",
			        ""address"": {
			            ""street1"": ""250 Alexis St"",
			            ""street2"": ""fake street"",
			            ""external_number"": ""91"",
			            ""city"": ""Red Deer"",
			            ""state"": ""Alberta"",
			            ""country"": ""CA"",
			            ""postal_code"": ""T4N 0B8""
			        }
			    }]
			}");

			FiscalEntity fiscal_entity = customer.createFiscalEntity(@"{
			    ""tax_id"": ""AMGH851205MN1"",
			    ""company_name"": ""Nike SA de CV"",
			    ""email"": ""contacto@nike.mx"",
			    ""phone"": ""+5215555555555"",
			    ""address"": {
			        ""street1"": ""250 Alexis St"",
			        ""street2"": ""fake street"",
			        ""external_number"": ""91"",
			        ""city"": ""Red Deer"",
			        ""state"": ""Alberta"",
			        ""country"": ""CA"",
			        ""postal_code"": ""T4N 0B8""
			    }
			}");

			Assert.AreEqual(fiscal_entity.tax_id, "AMGH851205MN1");
		}

		[Test()]
		public void updateFiscalEntity()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com"",
			    ""corporate"": true
			}");

			FiscalEntity fiscal_entity = customer.createFiscalEntity(@"{
			    ""tax_id"": ""AMGH851205MN1"",
			    ""name"": ""Nike SA de CV"",
			    ""email"": ""contacto@nike.mx"",
			    ""address"": {
			        ""street1"": ""250 Alexis St"",
			        ""street2"": ""fake street"",
			        ""external_number"": ""91"",
			        ""city"": ""Red Deer"",
			        ""state"": ""Alberta"",
			        ""country"": ""CA"",
			        ""postal_code"": ""T4N 0B8""
			    }
			}");

			Assert.AreEqual(fiscal_entity.tax_id, "AMGH851205MN1");

			fiscal_entity = fiscal_entity.update(@"{
				""tax_id"": ""CECC881116KP3""	
			}");

			Assert.AreEqual(fiscal_entity.tax_id, "CECC881116KP3");
		}

		[Test()]
		public void deleteFiscalEntity()
		{
			conekta.Api.apiKey = "key_eYvWV7gSDkNYXsmr";
			conekta.Api.version = "2.0.0";

			Customer customer = new conekta.Customer().create(@"{
			    ""name"": ""Emiliano Cabrera"",
			    ""phone"": ""+5215544443333"",
			    ""email"": ""user@example.com"",
			    ""corporate"": true
			}");

			FiscalEntity fiscal_entity = customer.createFiscalEntity(@"{
			    ""tax_id"": ""AMGH851205MN1"",
			    ""company_name"": ""Nike SA de CV"",
			    ""email"": ""contacto@nike.mx"",
			    ""phone"": ""+5215555555555"",
			    ""address"": {
			        ""street1"": ""250 Alexis St"",
			        ""street2"": ""fake street"",
			        ""external_number"": ""91"",
			        ""city"": ""Red Deer"",
			        ""state"": ""Alberta"",
			        ""country"": ""CA"",
			        ""postal_code"": ""T4N 0B8""
			    }
			}");

			Assert.AreEqual(fiscal_entity.tax_id, "AMGH851205MN1");

			fiscal_entity = fiscal_entity.update(@"{
				""tax_id"": ""CECC881116KP3""
			}");

			Assert.AreEqual(fiscal_entity.tax_id, "CECC881116KP3");

			fiscal_entity = fiscal_entity.destroy();

			Assert.AreEqual(fiscal_entity.tax_id, "CECC881116KP3");
		}
	}
}
