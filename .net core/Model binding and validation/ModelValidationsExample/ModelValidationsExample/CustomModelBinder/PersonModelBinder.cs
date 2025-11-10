using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModelValidationsExample.Models;

namespace ModelValidationsExample.CustomModelBinder
{
	public class PersonModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			Person person = new Person();
			//FirstName and LastName
			if (bindingContext.ValueProvider.GetValue("FirstName").Length > 0)
			{
				string firstName = bindingContext.ValueProvider.GetValue("FirstName").FirstValue;

				if (bindingContext.ValueProvider.GetValue("FirstName").Count() > 0)
				{
					person.PersonName += bindingContext.ValueProvider.GetValue("FirstName").FirstValue;

				}
			}
			//Bind other properties 
			//Email
			if (bindingContext.ValueProvider.GetValue("FirstName").Count() > 0)
				person.Email = bindingContext.ValueProvider.GetValue("Email").FirstValue;
			//bind all other properties.....



			bindingContext.Result = ModelBindingResult.Success(person);
			return Task.CompletedTask;

		}
	}
}
