using System;
using TMPro;
using UnityEngine;

public class Password : MonoBehaviour
{
    public TMP_InputField enteredPassword;
    public int hours;
    public int minutes;

    /* The correct answer is in the format 'XXabXXcdXXef'
       where 'X' represents a random character, 'ab' should 
       be the current hours of the system (if its value 
       is > 12, we subtract 12 from that), 'cd' should be
       the current minutes of the system, 'ef' should be
       the sum of 'ab' and 'cd' */
    public void Enter()                             //called when the player clicks 'enter' after entering a password
    {
        hours = DateTime.Now.Hour;                  //stores the current hours of this system
        minutes = DateTime.Now.Minute;              //stores the current minutes of this system
        
        if(hours > 12)
            hours -= 12;
        
        //conditions to check the validity of the password
        if (enteredPassword.text.Length >= 12)
        {
            int enteredHours;
            int enteredMinutes;
            int enteredSum;
            if(int.TryParse(enteredPassword.text.Substring(2, 2), out enteredHours) && 
               int.TryParse(enteredPassword.text.Substring(6, 2), out enteredMinutes) &&
               int.TryParse(enteredPassword.text.Substring(10, 2), out enteredSum))
            {
                if ((enteredHours == hours) && (Math.Abs(minutes - enteredMinutes) <= 5) &&
                    (enteredSum == enteredHours + enteredMinutes))
                    PlayerValues.Instance.isUnlocked = true;
            }
        }
        
        PlayerValues.Instance.SaveValues();
        
        //opening the home screen once the user has entered the right password
        if(PlayerValues.Instance.isUnlocked)
            MenuManager.Instance.OpenMenu("HomeScreen");
    }
}
