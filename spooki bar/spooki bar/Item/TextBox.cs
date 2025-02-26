﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Text;
using System.Windows;
using System.Threading;

public delegate void TextBoxEvent(TextBox sender);

public class TextBox : IKeyboardSubscriber
{
    Texture2D _textBoxTexture;
    Texture2D _caretTexture;

    SpriteFont _font;

    public void Dispose()
    {
        _textBoxTexture.Dispose();
        _caretTexture.Dispose();
        _font = null;
        this._text = null;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; private set; }

    public bool Highlighted { get; set; }

    public bool PasswordBox { get; set; }

    public event TextBoxEvent Clicked;

    string _text = "";
    public String Text
    {
        get
        {
            return _text;
        }
        set
        {
            if (_font != null)
            {
                _text = value;
                if (_text == null)
                    _text = "";

                if (_text != "")
                {
                    //if you attempt to display a character that is not in your font
                    //you will get an exception, so we filter the characters
                    //remove the filtering if you're using a default character in your spritefont
                    String filtered = "";
                    foreach (char c in value)
                    {
                        if (_font.Characters.Contains(c))
                            filtered += c;
                    }

                    _text = filtered;

                    while (_font.MeasureString(_text).X > Width)
                    {
                        //to ensure that text cannot be larger than the box
                        _text = _text.Substring(0, _text.Length - 1);
                    }
                }
            }
        }
    }

    public TextBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font)
    {
        _textBoxTexture = textBoxTexture;
        _caretTexture = caretTexture;
        _font = font;
        Height = (int)font.MeasureString("A").Y;
        _previousMouse = Mouse.GetState();
    }

    MouseState _previousMouse;
    public void Update(GameTime gameTime)
    {
        MouseState mouse = Mouse.GetState();
        Point mousePoint = new Point(mouse.X, mouse.Y);

        Rectangle position = new Rectangle(X, Y, Width, Height);
        if (position.Contains(mousePoint))
        {
            Highlighted = true;
            if (_previousMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed)
            {
                if (Clicked != null)
                    Clicked(this);
            }
        }
        else
        {
            Highlighted = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        bool caretVisible = true;

        if ((gameTime!=null) && (gameTime.TotalGameTime.TotalMilliseconds % 1000) < 500)
            caretVisible = false;
        else
            caretVisible = true;

        String toDraw = Text;

        if (PasswordBox)
        {
            toDraw = "";
            for (int i = 0; i < Text.Length; i++)
                toDraw += '*'; //bullet character (make sure you include it in the font!!!!)
        }

        //my texture was split vertically in 2 parts, upper was unhighlighted, lower was highlighted version of the box
        spriteBatch.Draw(_textBoxTexture, new Rectangle(X, Y, Width, Height), new Rectangle(0, Highlighted ? (_textBoxTexture.Height / 2) : 0, _textBoxTexture.Width, _textBoxTexture.Height / 2), Color.White);

        Vector2 size;
             size = _font.MeasureString(toDraw);

        if (caretVisible && Selected)
            spriteBatch.Draw(_caretTexture, new Rectangle(5 + X + (int)size.X + 2, Y + 2, 20, Height), Color.White); //my caret texture was a simple vertical line, 4 pixels smaller than font size.Y

        //shadow first, then the actual text
        spriteBatch.DrawString(_font, toDraw, new Vector2(5 + X, Y + 2) + Vector2.One, Color.Black);
        spriteBatch.DrawString(_font, toDraw, new Vector2(5 + X, Y), Color.White);
    }

    public void RecieveTextInput(char inputChar)
    {
        Text = Text + inputChar;
    }
    public void RecieveTextInput(string text)
    {
        Text = Text + text;
    }
    public void RecieveCommandInput(char command)
    {
        switch (command)
        {
            case '\b': //backspace
                if (Text.Length > 0)
                    Text = Text.Substring(0, Text.Length - 1);
                break;
            case '\r': //return
                if (OnEnterPressed != null)
                    OnEnterPressed(this);
                break;
            case '\t': //tab
                if (OnTabPressed != null)
                    OnTabPressed(this);
                break;
            default:
                break;
        }
    }
    public void RecieveSpecialInput(Keys key)
    {

    }

    public event TextBoxEvent OnEnterPressed;
    public event TextBoxEvent OnTabPressed;

    public bool Selected
    {
        get;
        set;
    }
}