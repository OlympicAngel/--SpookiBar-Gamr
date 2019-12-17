using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Windows.Input;

public interface IKeyboardSubscriber
{
    void RecieveTextInput(char inputChar);
    void RecieveTextInput(string text);
    void RecieveCommandInput(char command);
    void RecieveSpecialInput(Keys key);

    bool Selected { get; set; } //or Focused
}