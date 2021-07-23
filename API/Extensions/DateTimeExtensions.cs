using System;

namespace API.Extensions
{
	public static class DateTimeExtensions
	{
		// Extension method to calculate age
		public static int CalculateAge(this DateTime dob) {
			var today = DateTime.Today; // 2021
			var age = today.Year - dob.Year; 

			if(dob.Date > today.AddYears(-age)) { // if user didn't have a birthday yet this year
				age--;
			}

			return age;
		} 
	}
}