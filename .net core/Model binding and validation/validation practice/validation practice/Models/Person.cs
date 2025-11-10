using System.ComponentModel.DataAnnotations;

namespace validation_practice.Models
{
    public class Person 
    {
        [Custom_Validators.CustomValidator]
        public string Name { get; set; }
    }
}
