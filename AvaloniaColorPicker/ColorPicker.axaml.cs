﻿/*
    AvaloniaColorPicker - A color picker for Avalonia.
    Copyright (C) 2020  Giorgio Bianchini
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 3.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace AvaloniaColorPicker
{
    /// <summary>
    /// A control that enables the user to select a <see cref="Color"/>.
    /// </summary>
    public class ColorPicker : UserControl
    {
        /// <summary>
        /// Set this property to <see langword="true"/> before creating any object from this library to disable all transitions.
        /// </summary>
        public static bool TransitionsDisabled { get; set; } = false;

        /// <summary>
        /// Resets the default palettes. This will restore any deleted palettes, and remove any custom colour from the default palettes. It will not affect any custom palette created by the user.
        /// </summary>
        public static void ResetDefaultPalettes()
        {
            System.IO.Directory.CreateDirectory(PaletteSelector.PaletteDirectory);

            using (System.IO.Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AvaloniaColorPicker.Palettes.Colorful.palette"))
            using (System.IO.Stream fileStream = System.IO.File.Create(System.IO.Path.Combine(PaletteSelector.PaletteDirectory, "Colorful.palette")))
            {
                resourceStream.CopyTo(fileStream);
            }

            using (System.IO.Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AvaloniaColorPicker.Palettes.IBM.palette"))
            using (System.IO.Stream fileStream = System.IO.File.Create(System.IO.Path.Combine(PaletteSelector.PaletteDirectory, "IBM.palette")))
            {
                resourceStream.CopyTo(fileStream);
            }

            using (System.IO.Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AvaloniaColorPicker.Palettes.Tol_Bright.palette"))
            using (System.IO.Stream fileStream = System.IO.File.Create(System.IO.Path.Combine(PaletteSelector.PaletteDirectory, "Tol_Bright.palette")))
            {
                resourceStream.CopyTo(fileStream);
            }

            using (System.IO.Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AvaloniaColorPicker.Palettes.Tol_Light.palette"))
            using (System.IO.Stream fileStream = System.IO.File.Create(System.IO.Path.Combine(PaletteSelector.PaletteDirectory, "Tol_Light.palette")))
            {
                resourceStream.CopyTo(fileStream);
            }

            using (System.IO.Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AvaloniaColorPicker.Palettes.Tol_Muted.palette"))
            using (System.IO.Stream fileStream = System.IO.File.Create(System.IO.Path.Combine(PaletteSelector.PaletteDirectory, "Tol_Muted.palette")))
            {
                resourceStream.CopyTo(fileStream);
            }

            using (System.IO.Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AvaloniaColorPicker.Palettes.Tol_Vibrant.palette"))
            using (System.IO.Stream fileStream = System.IO.File.Create(System.IO.Path.Combine(PaletteSelector.PaletteDirectory, "Tol_Vibrant.palette")))
            {
                resourceStream.CopyTo(fileStream);
            }

            using (System.IO.Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AvaloniaColorPicker.Palettes.Wong.palette"))
            using (System.IO.Stream fileStream = System.IO.File.Create(System.IO.Path.Combine(PaletteSelector.PaletteDirectory, "Wong.palette")))
            {
                resourceStream.CopyTo(fileStream);
            }
        }

        internal enum ColorSpaces
        {
            RGB, HSB, LAB
        }

        internal enum ColorComponents
        {
            R, G, B, H, S, L, A
        }

        private byte R
        {
            get
            {
                return (byte)this.FindControl<NumericUpDown>("R_NUD").Value;
            }

            set
            {
                this.FindControl<NumericUpDown>("R_NUD").Value = value;
            }
        }

        private byte G
        {
            get
            {
                return (byte)this.FindControl<NumericUpDown>("G_NUD").Value;
            }

            set
            {
                this.FindControl<NumericUpDown>("G_NUD").Value = value;
            }
        }

        private byte B
        {
            get
            {
                return (byte)this.FindControl<NumericUpDown>("B_NUD").Value;
            }

            set
            {
                this.FindControl<NumericUpDown>("B_NUD").Value = value;
            }
        }

        private byte A
        {
            get
            {
                return (byte)this.FindControl<NumericUpDown>("A_NUD").Value;
            }

            set
            {
                this.FindControl<NumericUpDown>("A_NUD").Value = value;
            }
        }

        private byte H
        {
            get
            {
                return (byte)this.FindControl<NumericUpDown>("H_NUD").Value;
            }

            set
            {
                this.FindControl<NumericUpDown>("H_NUD").Value = value;
            }
        }

        private byte S
        {
            get
            {
                return (byte)this.FindControl<NumericUpDown>("S_NUD").Value;
            }

            set
            {
                this.FindControl<NumericUpDown>("S_NUD").Value = value;
            }
        }

        private byte V
        {
            get
            {
                return (byte)this.FindControl<NumericUpDown>("V_NUD").Value;
            }

            set
            {
                this.FindControl<NumericUpDown>("V_NUD").Value = value;
            }
        }

        private byte L
        {
            get
            {
                return (byte)this.FindControl<NumericUpDown>("L_NUD").Value;
            }

            set
            {
                byte val = Math.Min(value, (byte)100);
                this.FindControl<NumericUpDown>("L_NUD").Value = val;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Stili di denominazione", Justification = "a* component is lowercase")]
        private int a
        {
            get
            {
                return (int)this.FindControl<NumericUpDown>("a_NUD").Value;
            }

            set
            {
                int val = Math.Max(-100, Math.Min(value, 100));
                this.FindControl<NumericUpDown>("a_NUD").Value = val;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Stili di denominazione", Justification = "b* component is lowercase")]
        private int b
        {
            get
            {
                return (int)this.FindControl<NumericUpDown>("b_NUD").Value;
            }

            set
            {
                int val = Math.Max(-100, Math.Min(value, 100));
                this.FindControl<NumericUpDown>("b_NUD").Value = val;
            }
        }



        private ColorSpaces ColorSpace { get; set; } = ColorSpaces.HSB;
        private ColorComponents ColorComponent { get; set; } = ColorComponents.H;

        /// <summary>
        /// The <see cref="Avalonia.Media.Color"/> that is currently selected.
        /// </summary>
        public Color Color
        {
            get { return Color.FromArgb(A, R, G, B); }
            set
            {
                ProgrammaticChange = true;
                SetColor(value.R, value.G, value.B, value.A);
                BuildColorInterface(false);
                ProgrammaticChange = false;
            }
        }

        /// <summary>
        /// Defines the <see cref="PreviousColor"/> property.
        /// </summary>
        public static readonly StyledProperty<Color?> PreviousColorProperty = AvaloniaProperty.Register<ColorPicker, Color?>(nameof(PreviousColor), null);

        /// <summary>
        /// Represents the previously selected <see cref="Avalonia.Media.Color"/> (e.g. if the <see cref="ColorPicker"/> is being used to change the colour of an object, it would represent the previous colour of the object. Set to <see langword="null" /> to hide the previous colour display.
        /// </summary>
        public Color? PreviousColor
        {
            get { return GetValue(PreviousColorProperty); }
            set { SetValue(PreviousColorProperty, value); }
        }

        private AnimatableColourCanvas AnimatableColourCanvas { get; }

        private void SetColor(byte r, byte g, byte b, byte a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;

            (double _L, double _a, double _b) = Lab.ToLab(R, G, B);

            this.L = (byte)Math.Min(100, Math.Max(0, (_L * 100)));
            this.a = (int)Math.Min(100, Math.Max(-100, (_a * 100)));
            this.b = (int)Math.Min(100, Math.Max(-100, (_b * 100)));

            (double _H, double _S, double _V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

            this.H = (byte)Math.Min(255, Math.Max(0, (_H * 255)));
            this.S = (byte)Math.Min(255, Math.Max(0, (_S * 255)));
            this.V = (byte)Math.Min(255, Math.Max(0, (_V * 255)));
        }

        /// <summary>
        /// Creates a new <see cref="ColorPicker"/> instance.
        /// </summary>
        public ColorPicker()
        {
            this.InitializeComponent();

            ((Style)((Styles)this.Styles[0])[4]).Setters.Add(new Setter(Path.StrokeProperty, Colours.BackgroundColour));

            this.FindControl<Border>("WarningTooltipBorder").BorderBrush = Colours.BorderLowColour;
            this.FindControl<Border>("WarningTooltipBorder").Background = Colours.BackgroundColour;

            IBrush alphaBGBrush = Brush.Parse("#DCDCDC");

            for (int i = 0; i < 256 / 8; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        Rectangle rect = new Rectangle() { Width = 8, Height = 8, Fill = alphaBGBrush, Margin = new Thickness(j * 8, i * 8, 0, 0) };
                        this.FindControl<Canvas>("AlphaCanvasBackground").Children.Add(rect);
                    }
                }
            }

            this.FindControl<ToggleButton>("Dim1Toggle").Checked += (s, e) =>
            {
                this.FindControl<ToggleButton>("Dim2Toggle").IsChecked = false;
                this.FindControl<ToggleButton>("Dim3Toggle").IsChecked = false;

                this.FindControl<ToggleButton>("Dim1Toggle").IsEnabled = false;
                this.FindControl<ToggleButton>("Dim2Toggle").IsEnabled = true;
                this.FindControl<ToggleButton>("Dim3Toggle").IsEnabled = true;

                switch (ColorSpace)
                {
                    case ColorSpaces.RGB:
                        ColorComponent = ColorComponents.R;
                        break;

                    case ColorSpaces.HSB:
                        ColorComponent = ColorComponents.H;
                        break;

                    case ColorSpaces.LAB:
                        ColorComponent = ColorComponents.L;
                        break;
                }

                BuildColorInterface(true);
            };

            this.FindControl<ToggleButton>("Dim2Toggle").Checked += (s, e) =>
            {
                this.FindControl<ToggleButton>("Dim1Toggle").IsChecked = false;
                this.FindControl<ToggleButton>("Dim3Toggle").IsChecked = false;

                this.FindControl<ToggleButton>("Dim1Toggle").IsEnabled = true;
                this.FindControl<ToggleButton>("Dim2Toggle").IsEnabled = false;
                this.FindControl<ToggleButton>("Dim3Toggle").IsEnabled = true;


                switch (ColorSpace)
                {
                    case ColorSpaces.RGB:
                        ColorComponent = ColorComponents.G;
                        break;

                    case ColorSpaces.HSB:
                        ColorComponent = ColorComponents.S;
                        break;

                    case ColorSpaces.LAB:
                        ColorComponent = ColorComponents.A;
                        break;
                }

                BuildColorInterface(true);
            };


            this.FindControl<ToggleButton>("Dim3Toggle").Checked += (s, e) =>
            {
                this.FindControl<ToggleButton>("Dim1Toggle").IsChecked = false;
                this.FindControl<ToggleButton>("Dim2Toggle").IsChecked = false;

                this.FindControl<ToggleButton>("Dim1Toggle").IsEnabled = true;
                this.FindControl<ToggleButton>("Dim2Toggle").IsEnabled = true;
                this.FindControl<ToggleButton>("Dim3Toggle").IsEnabled = false;

                ColorComponent = ColorComponents.B;

                BuildColorInterface(true);
            };

            this.FindControl<ComboBox>("ColorSpaceComboBox").SelectionChanged += (s, e) =>
            {
                switch (this.FindControl<ComboBox>("ColorSpaceComboBox").SelectedIndex)
                {
                    case 0:
                        ColorSpace = ColorSpaces.RGB;
                        this.FindControl<ToggleButton>("Dim1Toggle").Content = "R";
                        this.FindControl<ToggleButton>("Dim2Toggle").Content = "G";
                        this.FindControl<ToggleButton>("Dim3Toggle").Content = "B";
                        break;
                    case 1:
                        ColorSpace = ColorSpaces.HSB;
                        this.FindControl<ToggleButton>("Dim1Toggle").Content = "H";
                        this.FindControl<ToggleButton>("Dim2Toggle").Content = "S";
                        this.FindControl<ToggleButton>("Dim3Toggle").Content = "B";
                        break;
                    case 2:
                        ColorSpace = ColorSpaces.LAB;
                        this.FindControl<ToggleButton>("Dim1Toggle").Content = "L*";
                        this.FindControl<ToggleButton>("Dim2Toggle").Content = "a*";
                        this.FindControl<ToggleButton>("Dim3Toggle").Content = "b*";
                        break;
                }

                if (this.FindControl<ToggleButton>("Dim1Toggle").IsChecked != true)
                {
                    this.FindControl<ToggleButton>("Dim1Toggle").IsChecked = true;
                }
                else
                {
                    this.FindControl<ToggleButton>("Dim1Toggle").IsChecked = false;
                    this.FindControl<ToggleButton>("Dim1Toggle").IsChecked = true;
                }

            };

            this.FindControl<Path>("HexagonColor1").PointerPressed += HexagonPressed;
            this.FindControl<Path>("HexagonColor2").PointerPressed += HexagonPressed;
            this.FindControl<Path>("HexagonColor3").PointerPressed += HexagonPressed;
            this.FindControl<Path>("HexagonColor4").PointerPressed += HexagonPressed;
            this.FindControl<Path>("HexagonColor5").PointerPressed += HexagonPressed;
            this.FindControl<Path>("HexagonColor7").PointerPressed += HexagonPressed;
            this.FindControl<Path>("HexagonColor8").PointerPressed += HexagonPressed;

            this.FindControl<NumericUpDown>("R_NUD").ValueChanged += UpdateColorFromNUD;
            this.FindControl<NumericUpDown>("G_NUD").ValueChanged += UpdateColorFromNUD;
            this.FindControl<NumericUpDown>("B_NUD").ValueChanged += UpdateColorFromNUD;

            this.FindControl<NumericUpDown>("H_NUD").ValueChanged += UpdateColorFromNUD;
            this.FindControl<NumericUpDown>("S_NUD").ValueChanged += UpdateColorFromNUD;
            this.FindControl<NumericUpDown>("V_NUD").ValueChanged += UpdateColorFromNUD;

            this.FindControl<NumericUpDown>("L_NUD").ValueChanged += UpdateColorFromNUD;
            this.FindControl<NumericUpDown>("a_NUD").ValueChanged += UpdateColorFromNUD;
            this.FindControl<NumericUpDown>("b_NUD").ValueChanged += UpdateColorFromNUD;

            this.FindControl<NumericUpDown>("A_NUD").ValueChanged += UpdateColorFromNUD;



            this.FindControl<TextBox>("Hex_Box").PropertyChanged += (s, e) =>
            {
                if (!ProgrammaticChange)
                {
                    if (e.Property == TextBox.TextProperty)
                    {
                        ProgrammaticChange = true;
                        try
                        {
                            string col = this.FindControl<TextBox>("Hex_Box").Text?.Trim();
                            if (col?.StartsWith("#") == true)
                            {
                                col = col.Substring(1);
                            }

                            if (col?.Length == 6 || col?.Length == 8)
                            {
                                string _R = col.Substring(0, 2);
                                string _G = col.Substring(2, 2);
                                string _B = col.Substring(4, 2);

                                string _A = col.Length > 6 ? col.Substring(6, 2) : "FF";

                                byte r = byte.Parse(_R, NumberStyles.HexNumber);
                                byte g = byte.Parse(_G, NumberStyles.HexNumber);
                                byte b = byte.Parse(_B, NumberStyles.HexNumber);
                                byte a = byte.Parse(_A, NumberStyles.HexNumber);

                                SetColor(r, g, b, a);

                                BuildColorInterface(false);
                            }
                        }
                        catch
                        {

                        }
                        ProgrammaticChange = false;
                    }
                }
            };

            this.FindControl<Canvas>("Canvas2D").PointerPressed += Canvas2DPressed;
            this.FindControl<Canvas>("Canvas2D").PointerReleased += Canvas2DReleased;
            this.FindControl<Canvas>("Canvas2D").PointerMoved += Canvas2DMove;

            this.FindControl<Canvas>("Canvas1D").PointerPressed += Canvas1DPressed;
            this.FindControl<Canvas>("Canvas1D").PointerReleased += Canvas1DReleased;
            this.FindControl<Canvas>("Canvas1D").PointerMoved += Canvas1DMove;

            this.FindControl<Canvas>("AlphaCanvas").PointerPressed += AlphaCanvasPressed;
            this.FindControl<Canvas>("AlphaCanvas").PointerReleased += AlphaCanvasReleased;
            this.FindControl<Canvas>("AlphaCanvas").PointerMoved += AlphaCanvasMove;

            AnimatableColourCanvas = new AnimatableColourCanvas();
            this.FindControl<Canvas>("Canvas2DImageContainer").Children.Add(AnimatableColourCanvas.Canvas2D);
            this.FindControl<Canvas>("Canvas1DImageContainer").Children.Add(AnimatableColourCanvas.Canvas1D);
            this.FindControl<Canvas>("AlphaCanvasImageContainer").Children.Add(AnimatableColourCanvas.AlphaCanvas);

            this.FindControl<PaletteSelector>("PaletteSelector").ColorSelected += (s, e) =>
            {
                Color col = e.Color;

                ProgrammaticChange = true;

                SetColor(col.R, col.G, col.B, col.A);
                BuildColorInterface(false);

                ProgrammaticChange = false;
            };

            this.FindControl<PaletteSelector>("PaletteSelector").Owner = this;

            this.FindControl<ComboBox>("ColourBlindnessBox").SelectionChanged += (s, e) =>
            {
                this.FindControl<PaletteSelector>("PaletteSelector").ColourBlindnessFunction = GetColourBlindnessFunction(this.FindControl<ComboBox>("ColourBlindnessBox").SelectedIndex);

                UpdateDependingOnAlpha(false);
            };

            if (TransitionsDisabled)
            {
                foreach (Style style in (Styles)Styles[0])
                {
                    List<Setter> toBeRemoved = new List<Setter>();

                    foreach (Setter setter in style.Setters)
                    {
                        if (setter.Property.Name == "Transitions")
                        {
                            toBeRemoved.Add(setter);
                        }
                    }

                    foreach (Setter setter in toBeRemoved)
                    {
                        style.Setters.Remove(setter);
                    }
                }
            }

            ProgrammaticChange = true;

            this.R = 0;
            this.G = 162;
            this.B = 232;

            this.H = 140;
            this.S = 255;
            this.V = 232;

            this.L = 63;
            this.a = -10;
            this.b = -44;

            this.A = 255;

            BuildColorInterface(true);

            ProgrammaticChange = false;
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == PreviousColorProperty)
            {
                UpdateDependingOnAlpha(true);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void BuildColorInterface(bool instantTransition)
        {
            WriteableBitmap canvas2Dimage;

            Canvas colorSpaceCanvas = null;

            Canvas previousCanvas = null;

            if (this.FindControl<Canvas>("ColorSpaceCanvas").Children.Count > 0)
            {
                previousCanvas = (Canvas)this.FindControl<Canvas>("ColorSpaceCanvas").Children[0];
            }

            switch (ColorSpace)
            {
                case ColorSpaces.RGB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.R:
                            canvas2Dimage = RGB.GetGB(R);
                            AnimatableColourCanvas.UpdateR(R, G, B, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Ellipse>("2DPositionCircle"), G, 255 - B, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Canvas>("1DPositionCanvas"), 0, 255 - R, instantTransition);
                            this.FindControl<Rectangle>("1DPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            colorSpaceCanvas = RGB.GetRCanvas(R, G, B, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                        case ColorComponents.G:
                            canvas2Dimage = RGB.GetRB(G);
                            AnimatableColourCanvas.UpdateG(R, G, B, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Ellipse>("2DPositionCircle"), 255 - R, 255 - B, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Canvas>("1DPositionCanvas"), 0, 255 - G, instantTransition);
                            this.FindControl<Rectangle>("1DPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            colorSpaceCanvas = RGB.GetGCanvas(R, G, B, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                        case ColorComponents.B:
                            canvas2Dimage = RGB.GetRG(B);
                            AnimatableColourCanvas.UpdateB(R, G, B, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Ellipse>("2DPositionCircle"), G, R, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Canvas>("1DPositionCanvas"), 0, 255 - B, instantTransition);
                            this.FindControl<Rectangle>("1DPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            colorSpaceCanvas = RGB.GetBCanvas(R, G, B, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                    }
                    if (S < 128 && V > 128)
                    {
                        this.FindControl<Ellipse>("2DPositionCircle").Stroke = Brushes.Black;
                    }
                    else
                    {
                        this.FindControl<Ellipse>("2DPositionCircle").Stroke = Brushes.White;
                    }
                    break;
                case ColorSpaces.HSB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.H:
                            canvas2Dimage = HSB.GetSB(H / 255.0);
                            AnimatableColourCanvas.UpdateH(H / 255.0, S / 255.0, V / 255.0, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Ellipse>("2DPositionCircle"), S, 255 - V, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Canvas>("1DPositionCanvas"), 0, 255 - H, instantTransition);
                            this.FindControl<Rectangle>("1DPositionRectangle").Fill = new SolidColorBrush(HSB.ColorFromHSV(H / 255.0, 1, 1));
                            colorSpaceCanvas = HSB.GetHCanvas(H / 255.0, S / 255.0, V / 255.0, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                        case ColorComponents.S:
                            canvas2Dimage = HSB.GetHB(S / 255.0);
                            AnimatableColourCanvas.UpdateS(H / 255.0, S / 255.0, V / 255.0, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Ellipse>("2DPositionCircle"), H, 255 - V, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Canvas>("1DPositionCanvas"), 0, 255 - S, instantTransition);
                            this.FindControl<Rectangle>("1DPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            colorSpaceCanvas = HSB.GetSCanvas(H / 255.0, S / 255.0, V / 255.0, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                        case ColorComponents.B:
                            canvas2Dimage = HSB.GetHS(V / 255.0);
                            AnimatableColourCanvas.UpdateV(H / 255.0, S / 255.0, V / 255.0, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Ellipse>("2DPositionCircle"), 128 + S * 0.5 * Math.Cos(H / 255.0 * 2 * Math.PI), 128 + S * 0.5 * Math.Sin(H / 255.0 * 2 * Math.PI), instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Canvas>("1DPositionCanvas"), 0, 255 - V, instantTransition);
                            this.FindControl<Rectangle>("1DPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            colorSpaceCanvas = HSB.GetBCanvas(H / 255.0, S / 255.0, V / 255.0, canvas2Dimage, previousCanvas, instantTransition);
                            break;
                    }
                    if (S < 128 && V > 128)
                    {
                        this.FindControl<Ellipse>("2DPositionCircle").Stroke = Brushes.Black;
                    }
                    else
                    {
                        this.FindControl<Ellipse>("2DPositionCircle").Stroke = Brushes.White;
                    }
                    break;
                case ColorSpaces.LAB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.L:
                            AnimatableColourCanvas.UpdateL(L / 100.0, a / 100.0, b / 100.0, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Ellipse>("2DPositionCircle"), (255 + 255 * b / 100.0) / 2, (255 - 255 * a / 100.0) / 2, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Canvas>("1DPositionCanvas"), 0, 255 - L / 100.0 * 255, instantTransition);
                            this.FindControl<Rectangle>("1DPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            colorSpaceCanvas = Lab.GetLCanvas(L / 100.0, a / 100.0, b / 100.0, previousCanvas, instantTransition);
                            break;
                        case ColorComponents.A:
                            AnimatableColourCanvas.Updatea(L / 100.0, a / 100.0, b / 100.0, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Ellipse>("2DPositionCircle"), (255 + 255 * b / 100.0) / 2, 255 - 255 * L / 100.0, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Canvas>("1DPositionCanvas"), 0, (255 - a / 100.0 * 255) * 0.5, instantTransition);
                            this.FindControl<Rectangle>("1DPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            colorSpaceCanvas = Lab.GetACanvas(L / 100.0, a / 100.0, b / 100.0, previousCanvas, instantTransition);
                            break;
                        case ColorComponents.B:
                            AnimatableColourCanvas.Updateb(L / 100.0, a / 100.0, b / 100.0, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Ellipse>("2DPositionCircle"), 255 - 255 * L / 100.0, (255 - 255 * a / 100.0) / 2, instantTransition);
                            SetTranslateRenderTransform(this.FindControl<Canvas>("1DPositionCanvas"), 0, (255 - b / 100.0 * 255) * 0.5, instantTransition);
                            this.FindControl<Rectangle>("1DPositionRectangle").Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                            colorSpaceCanvas = Lab.GetBCanvas(L / 100.0, a / 100.0, b / 100.0, previousCanvas, instantTransition);
                            break;
                    }

                    (double L1, double a1, double b1) = Lab.ToLab(R, G, B);

                    double C = Math.Sqrt(a1 * a1 + b1 * b1);
                    double Sl = C / Math.Sqrt(C * C + L1 * L1);

                    if (Sl < 0.5 && L1 > 0.5)
                    {
                        this.FindControl<Ellipse>("2DPositionCircle").Stroke = Brushes.Black;
                    }
                    else
                    {
                        this.FindControl<Ellipse>("2DPositionCircle").Stroke = Brushes.White;
                    }
                    break;
            }

            UpdateDependingOnAlpha(instantTransition);

            if (colorSpaceCanvas != null && colorSpaceCanvas != previousCanvas)
            {
                this.FindControl<Canvas>("ColorSpaceCanvas").Children.Clear();
                colorSpaceCanvas.ClipToBounds = true;
                this.FindControl<Canvas>("ColorSpaceCanvas").Children.Add(colorSpaceCanvas);
            }
        }


        internal static void SetTranslateRenderTransform(Control control, double X, double Y, bool instantTransition)
        {
            if (instantTransition)
            {
                control.RenderTransform = GetTranslateTransform(X, Y);
            }
            else
            {
                ((TranslateTransform)control.RenderTransform).X = X;
                ((TranslateTransform)control.RenderTransform).Y = Y;
            }
        }

        internal static TranslateTransform GetTranslateTransform(double X, double Y)
        {
            TranslateTransform tbr = new TranslateTransform() { X = X, Y = Y };

            if (!ColorPicker.TransitionsDisabled)
            {
                tbr.Transitions = new Avalonia.Animation.Transitions
                {
                    new DoubleTransition() { Property = TranslateTransform.XProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) },
                    new DoubleTransition() { Property = TranslateTransform.YProperty, Duration = new TimeSpan(0, 0, 0, 0, 100) }
                };
            }
            return tbr;
        }

        private static Func<Color, Color> GetColourBlindnessFunction(int index)
        {
            switch (index)
            {
                case 1:
                    return ColourBlindness.Protanopia;
                case 2:
                    return ColourBlindness.Deuteranopia;
                case 3:
                    return ColourBlindness.Tritanopia;
                case 4:
                    return ColourBlindness.ConeAchromatopsia;
                case 5:
                    return ColourBlindness.RodAchromatopsia;
                case 0:
                default:
                    return ColourBlindness.NormalVision;
            }
        }

        private void UpdateDependingOnAlpha(bool instantTransition)
        {
            SetTranslateRenderTransform(this.FindControl<Canvas>("AlphaPositionCanvas"), 0, 255 - A, instantTransition);

            Color currCol = Color.FromArgb(A, R, G, B);

            this.FindControl<Rectangle>("AlphaPositionRectangle").Fill = new SolidColorBrush(currCol);

            Func<Color, Color> colourBlindnessFunction = GetColourBlindnessFunction(this.FindControl<ComboBox>("ColourBlindnessBox").SelectedIndex);

            Color prevContrasting = Colors.Transparent;
            Color prevLight = Colors.Transparent;
            Color prevDark = Colors.Transparent;

            if (PreviousColor != null)
            {
                prevContrasting = GetContrastingColor(PreviousColor.Value);
                prevLight = GetLighterColor(PreviousColor.Value);
                prevDark = GetDarkerColor(PreviousColor.Value);
                this.FindControl<Path>("HexagonColor1").IsVisible = true;
                this.FindControl<Path>("HexagonColor2").IsVisible = true;
                this.FindControl<Path>("HexagonColor3").IsVisible = true;
                this.FindControl<Path>("HexagonColor4").IsVisible = true;
                this.FindControl<TextBlock>("CurrentBlock").IsVisible = true;
                this.FindControl<TextBlock>("NewBlock").IsVisible = true;
            }
            else
            {
                this.FindControl<TextBlock>("CurrentBlock").IsVisible = false;
                this.FindControl<TextBlock>("NewBlock").IsVisible = false;
                this.FindControl<Path>("HexagonColor1").IsVisible = false;
                this.FindControl<Path>("HexagonColor2").IsVisible = false;
                this.FindControl<Path>("HexagonColor3").IsVisible = false;
                this.FindControl<Path>("HexagonColor4").IsVisible = false;
            }

            Color contrasting = GetContrastingColor(currCol);
            Color light = GetLighterColor(currCol);
            Color dark = GetDarkerColor(currCol);

            if (instantTransition)
            {
                if (PreviousColor != null)
                {
                    this.FindControl<Path>("HexagonColor1").Fill = new ColorVisualBrush(colourBlindnessFunction(prevContrasting));
                    this.FindControl<Path>("HexagonColor1").Tag = prevContrasting;
                    this.FindControl<Path>("HexagonColor2").Fill = new ColorVisualBrush(colourBlindnessFunction(PreviousColor.Value));
                    this.FindControl<Path>("HexagonColor2").Tag = PreviousColor;
                    this.FindControl<Path>("HexagonColor3").Fill = new ColorVisualBrush(colourBlindnessFunction(prevLight));
                    this.FindControl<Path>("HexagonColor3").Tag = prevLight;
                    this.FindControl<Path>("HexagonColor4").Fill = new ColorVisualBrush(colourBlindnessFunction(prevDark));
                    this.FindControl<Path>("HexagonColor4").Tag = prevDark;
                }

                this.FindControl<Path>("HexagonColor5").Fill = new ColorVisualBrush(colourBlindnessFunction(contrasting));
                this.FindControl<Path>("HexagonColor5").Tag = contrasting;
                this.FindControl<Path>("HexagonColor6").Fill = new ColorVisualBrush(colourBlindnessFunction(currCol));
                this.FindControl<Path>("HexagonColor6").Tag = currCol;
                this.FindControl<Path>("HexagonColor7").Fill = new ColorVisualBrush(colourBlindnessFunction(light));
                this.FindControl<Path>("HexagonColor7").Tag = light;
                this.FindControl<Path>("HexagonColor8").Fill = new ColorVisualBrush(colourBlindnessFunction(dark));
                this.FindControl<Path>("HexagonColor8").Tag = dark;
            }
            else
            {
                if (PreviousColor != null)
                {
                    ((ColorVisualBrush)this.FindControl<Path>("HexagonColor1").Fill).Color = colourBlindnessFunction(prevContrasting);
                    this.FindControl<Path>("HexagonColor1").Tag = prevContrasting;
                    ((ColorVisualBrush)this.FindControl<Path>("HexagonColor2").Fill).Color = colourBlindnessFunction(PreviousColor.Value);
                    this.FindControl<Path>("HexagonColor2").Tag = PreviousColor;
                    ((ColorVisualBrush)this.FindControl<Path>("HexagonColor3").Fill).Color = colourBlindnessFunction(prevLight);
                    this.FindControl<Path>("HexagonColor3").Tag = prevLight;
                    ((ColorVisualBrush)this.FindControl<Path>("HexagonColor4").Fill).Color = colourBlindnessFunction(prevDark);
                    this.FindControl<Path>("HexagonColor4").Tag = prevDark;
                }

                ((ColorVisualBrush)this.FindControl<Path>("HexagonColor5").Fill).Color = colourBlindnessFunction(contrasting);
                this.FindControl<Path>("HexagonColor5").Tag = contrasting;
                ((ColorVisualBrush)this.FindControl<Path>("HexagonColor6").Fill).Color = colourBlindnessFunction(currCol);
                this.FindControl<Path>("HexagonColor6").Tag = currCol;
                ((ColorVisualBrush)this.FindControl<Path>("HexagonColor7").Fill).Color = colourBlindnessFunction(light);
                this.FindControl<Path>("HexagonColor7").Tag = light;
                ((ColorVisualBrush)this.FindControl<Path>("HexagonColor8").Fill).Color = colourBlindnessFunction(dark);
                this.FindControl<Path>("HexagonColor8").Tag = dark;
            }

            string hexColor = R.ToString("X2");
            hexColor += G.ToString("X2");
            hexColor += B.ToString("X2");

            if (A < 255)
            {
                hexColor += A.ToString("X2");
            }

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ProgrammaticChange = true;
                this.FindControl<TextBox>("Hex_Box").Text = hexColor;
                ProgrammaticChange = false;
            });
        }

        internal static Color GetContrastingColor(Color col)
        {
            (double L1, double a1, double b1) = Lab.ToLab(col);

            (double L2, double a2, double b2) = Lab.GetContrastingColor(L1, a1, b1);

            Lab.FromLab(L2, a2, b2, out byte R, out byte G, out byte B);

            return Color.FromArgb(col.A, R, G, B);
        }

        internal static Color GetDarkerColor(Color col)
        {
            (double L1, double a1, double b1) = Lab.ToLab(col);

            Lab.FromLab(L1 - 0.15, a1, b1, out byte R, out byte G, out byte B);

            return Color.FromArgb(col.A, R, G, B);
        }

        internal static Color GetLighterColor(Color col)
        {
            (double L1, double a1, double b1) = Lab.ToLab(col);

            Lab.FromLab(L1 + 0.15, a1, b1, out byte R, out byte G, out byte B);

            return Color.FromArgb(col.A, R, G, B);
        }


        private bool ProgrammaticChange = false;

        bool IsCanvas2DPressed = false;

        private DispatcherTimer Canvas2DTimer { get; } = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Input, StopTimer);
        private DispatcherTimer Canvas1DTimer { get; } = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Input, StopTimer);
        private DispatcherTimer AlphaCanvasTimer { get; } = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Input, StopTimer);

        private static void StopTimer(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
        }

        private void UpdateColorFromCanvas2D(Point pos, bool instantTransition)
        {
            ProgrammaticChange = true;

            double x = Math.Max(0, Math.Min(pos.X, 255));
            double y = Math.Max(0, Math.Min(pos.Y, 255));

            byte R = this.R;
            byte G = this.G;
            byte B = this.B;

            switch (ColorSpace)
            {
                case ColorSpaces.RGB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.R:
                            G = (byte)Math.Round(x);
                            B = (byte)Math.Round(255 - y);
                            break;
                        case ColorComponents.G:
                            R = (byte)Math.Round(255 - x);
                            B = (byte)Math.Round(255 - y);
                            break;
                        case ColorComponents.B:
                            R = (byte)Math.Round(y);
                            G = (byte)Math.Round(x);
                            break;
                    }
                    break;

                case ColorSpaces.HSB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.H:
                            S = (byte)x;
                            V = (byte)(255 - y);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                        case ColorComponents.S:
                            H = (byte)x;
                            V = (byte)(255 - y);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                        case ColorComponents.B:
                            double h = Math.Atan2(y - 128, x - 128);
                            if (h < 0)
                            {
                                h += 2 * Math.PI;
                            }
                            h /= 2 * Math.PI;
                            H = (byte)(h * 255);
                            S = (byte)(Math.Min(1, Math.Sqrt((x - 128) * (x - 128) + (y - 128) * (y - 128)) / 128) * 255);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                    }

                    break;
                case ColorSpaces.LAB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.L:
                            a = (int)(100 - 2 * y / 255.0 * 100);
                            b = (int)(2 * x / 255.0 * 100 - 100);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                        case ColorComponents.A:
                            L = (byte)(100 - y / 255.0 * 100);
                            b = (int)(2 * x / 255.0 * 100 - 100);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                        case ColorComponents.B:
                            L = (byte)(100 - x / 255.0 * 100);
                            a = (int)(100 - 2 * y / 255.0 * 100);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                    }
                    break;
            }

            this.R = R;
            this.G = G;
            this.B = B;

            if (ColorSpace != ColorSpaces.LAB)
            {
                (double _L, double _a, double _b) = Lab.ToLab(R, G, B);

                this.L = (byte)Math.Min(100, Math.Max(0, (_L * 100)));
                this.a = (int)Math.Min(100, Math.Max(-100, (_a * 100)));
                this.b = (int)Math.Min(100, Math.Max(-100, (_b * 100)));
            }

            if (ColorSpace != ColorSpaces.HSB)
            {
                (double _H, double _S, double _V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

                this.H = (byte)Math.Min(255, Math.Max(0, (_H * 255)));
                this.S = (byte)Math.Min(255, Math.Max(0, (_S * 255)));
                this.V = (byte)Math.Min(255, Math.Max(0, (_V * 255)));
            }

            BuildColorInterface(instantTransition);

            ProgrammaticChange = false;
        }

        private void Canvas2DPressed(object sender, PointerPressedEventArgs e)
        {
            Point pos = e.GetPosition(this.FindControl<Canvas>("Canvas2D"));

            UpdateColorFromCanvas2D(pos, false);

            IsCanvas2DPressed = true;

            Canvas2DTimer.Start();
        }

        private void Canvas2DReleased(object sender, PointerReleasedEventArgs e)
        {
            IsCanvas2DPressed = false;
        }

        private void Canvas2DMove(object sender, PointerEventArgs e)
        {
            if (IsCanvas2DPressed && !Canvas2DTimer.IsEnabled)
            {
                Point pos = e.GetPosition(this.FindControl<Canvas>("Canvas2D"));

                UpdateColorFromCanvas2D(pos, true);
            }
        }



        bool IsCanvas1DPressed = false;

        private void UpdateColorFromCanvas1D(Point pos, bool instantTransition)
        {
            ProgrammaticChange = true;

            double y = Math.Max(0, Math.Min(pos.Y, 255));

            byte R = this.R;
            byte G = this.G;
            byte B = this.B;

            switch (ColorSpace)
            {
                case ColorSpaces.RGB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.R:
                            R = (byte)Math.Round(255 - y);
                            break;
                        case ColorComponents.G:
                            G = (byte)Math.Round(255 - y);
                            break;
                        case ColorComponents.B:
                            B = (byte)Math.Round(255 - y);
                            break;
                    }
                    break;

                case ColorSpaces.HSB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.H:
                            H = (byte)(255 - y);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                        case ColorComponents.S:
                            S = (byte)(255 - y);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                        case ColorComponents.B:
                            V = (byte)(255 - y);
                            HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out R, out G, out B);
                            break;
                    }
                    break;

                case ColorSpaces.LAB:
                    switch (ColorComponent)
                    {
                        case ColorComponents.L:
                            L = (byte)(100 - y / 255.0 * 100.0);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                        case ColorComponents.A:
                            a = (int)(100 - 2 * y / 255.0 * 100.0);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                        case ColorComponents.B:
                            b = (int)(100 - 2 * y / 255.0 * 100.0);
                            Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out R, out G, out B);
                            break;
                    }
                    break;
            }

            this.R = R;
            this.G = G;
            this.B = B;

            if (ColorSpace != ColorSpaces.LAB)
            {
                (double _L, double _a, double _b) = Lab.ToLab(R, G, B);

                this.L = (byte)Math.Min(100, Math.Max(0, (_L * 100)));
                this.a = (int)Math.Min(100, Math.Max(-100, (_a * 100)));
                this.b = (int)Math.Min(100, Math.Max(-100, (_b * 100)));
            }

            if (ColorSpace != ColorSpaces.HSB)
            {
                (double _H, double _S, double _V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

                this.H = (byte)Math.Min(255, Math.Max(0, (_H * 255)));
                this.S = (byte)Math.Min(255, Math.Max(0, (_S * 255)));
                this.V = (byte)Math.Min(255, Math.Max(0, (_V * 255)));
            }

            BuildColorInterface(instantTransition);

            ProgrammaticChange = false;
        }

        private void Canvas1DPressed(object sender, PointerPressedEventArgs e)
        {
            Point pos = e.GetPosition(this.FindControl<Canvas>("Canvas1D"));

            UpdateColorFromCanvas1D(pos, false);

            IsCanvas1DPressed = true;
            Canvas1DTimer.Start();
        }

        private void Canvas1DReleased(object sender, PointerReleasedEventArgs e)
        {
            IsCanvas1DPressed = false;
        }

        private void Canvas1DMove(object sender, PointerEventArgs e)
        {
            if (IsCanvas1DPressed && !Canvas1DTimer.IsEnabled)
            {
                Point pos = e.GetPosition(this.FindControl<Canvas>("Canvas1D"));

                UpdateColorFromCanvas1D(pos, true);
            }
        }

        bool IsAlphaCanvasPressed = false;

        private void UpdateColorFromAlphaCanvas(Point pos, bool instantTransition)
        {
            ProgrammaticChange = true;

            double y = Math.Max(0, Math.Min(pos.Y, 255));

            this.A = (byte)Math.Round(255 - y);

            BuildColorInterface(instantTransition);

            ProgrammaticChange = false;
        }

        private void AlphaCanvasPressed(object sender, PointerPressedEventArgs e)
        {
            Point pos = e.GetPosition(this.FindControl<Canvas>("AlphaCanvas"));

            UpdateColorFromAlphaCanvas(pos, false);

            IsAlphaCanvasPressed = true;

            AlphaCanvasTimer.Start();
        }

        private void AlphaCanvasReleased(object sender, PointerReleasedEventArgs e)
        {
            IsAlphaCanvasPressed = false;
        }

        private void AlphaCanvasMove(object sender, PointerEventArgs e)
        {
            if (IsAlphaCanvasPressed && !AlphaCanvasTimer.IsEnabled)
            {
                Point pos = e.GetPosition(this.FindControl<Canvas>("AlphaCanvas"));

                UpdateColorFromAlphaCanvas(pos, true);
            }
        }

        private void UpdateColorFromNUD(object sender, NumericUpDownValueChangedEventArgs e)
        {
            if (!ProgrammaticChange)
            {
                ProgrammaticChange = true;

                ColorSpaces? currColorSpace = null;

                switch (((NumericUpDown)sender).Name)
                {
                    case "R_NUD":
                    case "G_NUD":
                    case "B_NUD":
                        currColorSpace = ColorSpaces.RGB;
                        break;

                    case "H_NUD":
                    case "S_NUD":
                    case "V_NUD":
                        currColorSpace = ColorSpaces.HSB;
                        byte _R, _G, _B;
                        HSB.HSVToRGB(H / 255.0, S / 255.0, V / 255.0, out _R, out _G, out _B);
                        this.R = _R;
                        this.G = _G;
                        this.B = _B;
                        break;

                    case "L_NUD":
                    case "a_NUD":
                    case "b_NUD":
                        currColorSpace = ColorSpaces.LAB;
                        Lab.FromLab(L / 100.0, a / 100.0, b / 100.0, out _R, out _G, out _B);
                        this.R = _R;
                        this.G = _G;
                        this.B = _B;
                        break;

                    case "A_NUD":
                        break;
                }

                if (currColorSpace != null)
                {
                    if (currColorSpace != ColorSpaces.LAB)
                    {
                        (double _L, double _a, double _b) = Lab.ToLab(R, G, B);

                        this.L = (byte)Math.Min(100, Math.Max(0, (_L * 100)));
                        this.a = (int)Math.Min(100, Math.Max(-100, (_a * 100)));
                        this.b = (int)Math.Min(100, Math.Max(-100, (_b * 100)));
                    }

                    if (currColorSpace != ColorSpaces.HSB)
                    {
                        (double _H, double _S, double _V) = HSB.RGBToHSB(R / 255.0, G / 255.0, B / 255.0);

                        this.H = (byte)Math.Min(255, Math.Max(0, (_H * 255)));
                        this.S = (byte)Math.Min(255, Math.Max(0, (_S * 255)));
                        this.V = (byte)Math.Min(255, Math.Max(0, (_V * 255)));
                    }
                    BuildColorInterface(false);
                }
                else
                {
                    UpdateDependingOnAlpha(false);
                }

                ProgrammaticChange = false;
            }
        }

        private void HexagonPressed(object sender, PointerPressedEventArgs e)
        {
            Color col = (Color)((Path)sender).Tag;

            ProgrammaticChange = true;

            SetColor(col.R, col.G, col.B, col.A);
            BuildColorInterface(false);

            ProgrammaticChange = false;
        }
    }
}
