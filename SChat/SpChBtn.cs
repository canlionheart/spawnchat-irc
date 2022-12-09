using SCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace SChat
{
    /*
     * WPF Buttons are a bit of a pain to get to look and work the way I wanted, so I did this instead.
     */
    public class SpChBtn
    {
        public Image btn { get; set; }
        public Label btnText { get; set; }
        public Label btnKey { get; set; }
        public string idleImage { get; set; }
        public string pressedImage { get; set; }
        public int fontSize { get; set; }
        public bool enabled = true;

        public SpChBtn(Image btn, Label btnText, Label btnKey, string idleImage, string pressedImage, int fontSize)
        {
            this.btn = btn;
            this.btnText = btnText;
            this.btnKey = btnKey;
            this.idleImage = idleImage;
            this.pressedImage = pressedImage;
            this.fontSize = fontSize;
        }

        public SpChBtn(Image btn, Label btnText, string idleImage, string pressedImage, int fontSize)
        {
            this.btn = btn;
            this.btnText = btnText;
            this.btnKey = null;
            this.idleImage = idleImage;
            this.pressedImage = pressedImage;
            this.fontSize = fontSize;
        }

        public SpChBtn(Image btn, string idleImage, string pressedImage)
        {
            this.btn = btn;
            this.idleImage = idleImage;
            this.pressedImage = pressedImage;
        }

        public void setImages(string idleImage, string pressedImage) {
            this.idleImage = idleImage;
            this.pressedImage = pressedImage;
        }

        public void press()
        {
                btn.Source = ResourceHelper.LoadBitmapFromResource("img/glues/" + pressedImage + ".png");
                if (btnText != null)
                {
                    btnText.FontSize = fontSize - 1;
                    if (btnKey != null)
                    {
                        btnKey.FontSize = fontSize - 1;
                    }
                }
            
        }

        public void dePress()
        {
            if (enabled)
            {
                btn.Source = ResourceHelper.LoadBitmapFromResource("img/glues/" + idleImage + ".png");
                if (btnText != null)
                {
                    btnText.FontSize = fontSize;
                    if (btnKey != null)
                    {
                        btnKey.FontSize = fontSize;
                    }
                }
            }
        }

        public void disable()
        {
            btn.IsEnabled = false;
            enabled = false;
            if (btnText != null) btnText.IsEnabled = false;
            if (btnKey != null) btnKey.IsEnabled = false;
            press();
        }

        public void enable()
        {
            btn.IsEnabled = true;
            enabled = true;
            if (btnText != null) btnText.IsEnabled = true;
            if (btnKey != null) btnKey.IsEnabled = true;
            dePress();
        }


    }
}
