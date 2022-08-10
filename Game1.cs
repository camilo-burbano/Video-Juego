using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;
using MonoGameDesktopGL;
using MonoGameDesktopGL.Content;


namespace MonoGameDesktopDX
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int width = 1200;
        int height = 600;
        Texture2D tex;
        Rectangle rect;
                
        bool colision = false;

        //ENEMIGOS
        List<Enemigo> listaEnemigos = new List<Enemigo>();
        Random random = new Random();
        int contador = 0;
        int limite = 30;

        //DISPAROS
        List<Disparo> listaDisparos = new List<Disparo>();
        Vector2 posicionDisparo;
        int contadorDisparos = 0;
        int limiteDisparos = 10;

        //MOUSE
        MouseState raton = new MouseState();

        //FONDO BACKGROUND
        Texture2D fondo;
        Rectangle rectFondo;

        //FUENTE
        SpriteFont fuente;

        //PUNTAJE
        int puntaje = 0;

        //VIDAS
        int vidas = 3;

        //ESFERAS
        List<Esfera> listaEsferas = new List<Esfera>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            VariablesVentana();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            CargarFondo();
            CargarFuente();
            CrearHeroe();
            //CrearEnemigosIniciales();

            // TODO: load content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (!colision)
            {
                MovimientosTeclado();
                ActualizarEnemigos();
                EliminarEnemigos();
                ComprobarVida();
                DetectarColision();
                Conteo();
                //DetectarRaton();
                MovimientosTeclado();
                DetectarDisparo();
                ActualizarDisparos();
                LimpiarDisparos();
                ReducidirVida();
                ActualizarEsferas();
                EliminarEsferas();
                ComprobarVidaEsfera();
                DetectarColisionEsfera();
                Finalizar();
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //TODO: Draw your game
            spriteBatch.Begin();
            spriteBatch.Draw(fondo, rectFondo, Color.White);
            DibujarEnemigos();
            DibujarDisparos();
            DibujarEsferas();
            spriteBatch.Draw(tex, rect, Color.White);
            spriteBatch.DrawString(fuente, "Puntaje: " + puntaje.ToString() + " Vidas: "+ vidas.ToString(), Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(time);
        }

        private void LimpiarDisparos()
        {
            for(int i = 0; i < listaDisparos.Count; i++)
            {
                if (!listaDisparos[i].estaVivo)
                {
                    listaDisparos.Remove(listaDisparos[i]);
                }
            }
        }
        private void DibujarDisparos()
        {
            for(int i = 0; i < listaDisparos.Count; i++)
            {
                listaDisparos[i].Draw();
            }
        }
        private void ActualizarDisparos()
        {
            for(int i = 0; i < listaDisparos.Count; i++)
            {
                listaDisparos[i].Update();
            }
        }
        private void Disparar()
        {
            posicionDisparo = new Vector2((rect.Width/2 + rect.X), (rect.Height / 2 + rect.Y));
            FileStream imagenDisparo = new FileStream("Content/ataque3.png", FileMode.Open);
            Texture2D texDisparo = Texture2D.FromStream(GraphicsDevice, imagenDisparo);
            imagenDisparo.Dispose();

            Disparo nuevoDisparo = new Disparo(texDisparo, posicionDisparo, spriteBatch);
            listaDisparos.Add(nuevoDisparo);
            GC.Collect();
        }
        private void DetectarDisparo()
        {
            contadorDisparos++;

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if(contadorDisparos >= limiteDisparos)
                {
                    Disparar();
                    contadorDisparos = 0;
                }                
            }
        }
        private void CargarFuente()
        {
            fuente = Content.Load<SpriteFont>("FONT");
        }
        private void CargarFondo()
        {
            FileStream imagenFondo = new FileStream("Content/fondo.png", FileMode.Open);
            fondo = Texture2D.FromStream(GraphicsDevice, imagenFondo);
            imagenFondo.Dispose();
            rectFondo = new Rectangle(0,0, 2000, height);
        }
        private void DetectarRaton()
        {
            raton = Mouse.GetState();
            rect.X = raton.Position.X;
            rect.Y = raton.Position.Y;
        }
        private void EliminarEnemigos()
        {
            for(int i = 0; i < listaEnemigos.Count; i++)
            {
                if(listaEnemigos[i].rect.X < 50)
                {
                    listaEnemigos[i].Eliminar();
                }
            }
        }
        private void ComprobarVida()
        {
            for(int i = 0; i < listaEnemigos.Count; i++)
            {
                if (!listaEnemigos[i].estaVivo)
                {
                    listaEnemigos.Remove(listaEnemigos[i]);
                }
            }
        }
        private void DibujarEnemigos()
        {
            for (int i = 0; i < listaEnemigos.Count; i++)
            {
                listaEnemigos[i].Draw();
            }
        }
        private void ActualizarEnemigos()
        {
            for (int i = 0; i < listaEnemigos.Count; i++)
            {
                listaEnemigos[i].Update();
            }
        }
        private void VariablesVentana()
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
            graphics.ApplyChanges();
        }
        private void CrearHeroe()
        {
            FileStream imagenChoco = new FileStream("Content/goku1.png", FileMode.Open);
            tex = Texture2D.FromStream(GraphicsDevice, imagenChoco);
            imagenChoco.Dispose();
            rect = new Rectangle(100, 300, 120, 120);
        }
        private void CrearEnemigosIniciales()
        {
            FileStream imagenEnemigo = new FileStream("Content/enemy1.png", FileMode.Open);
            for (int i = 0; i < 5; i++)
            {
                Texture2D texEnemigo = Texture2D.FromStream(GraphicsDevice, imagenEnemigo);
                Vector2 posicionEnemigo = new Vector2(random.Next(0, width), -200);
                Vector2 velocidadEnemigo = new Vector2(0, random.Next(1, 3));
                Enemigo nuevoEnemigo = new Enemigo(
                    texEnemigo,
                    posicionEnemigo,
                    velocidadEnemigo,
                    spriteBatch);

                listaEnemigos.Add(nuevoEnemigo);
                GC.Collect();
            }
            imagenEnemigo.Dispose();
        }
        private void Conteo()
        {
            contador++;
            if(contador >= limite)
            {
                CrearEnemigo();
                CrearEsfera();
                contador = 0;
            }
        }
        private void CrearEnemigo()
        {
            int texAUsar = random.Next(0, 3);
            FileStream imagenEnemigo;

            if (texAUsar == 0)
            {
                imagenEnemigo = new FileStream("Content/saibaman.png", FileMode.Open);
            }
            else if (texAUsar == 1)
            {
                imagenEnemigo = new FileStream("Content/cellJunior.png", FileMode.Open);
            }
            else
            {
                imagenEnemigo = new FileStream("Content/metalCooler.png", FileMode.Open);
            }

            
            Texture2D texEnemigo = Texture2D.FromStream(GraphicsDevice, imagenEnemigo);
            Vector2 posicionEnemigo = new Vector2(1150, random.Next(0, height));
            Vector2 velocidadEnemigo = new Vector2(0, random.Next(1, 3));
            Enemigo nuevoEnemigo = new Enemigo(
                 texEnemigo,
                 posicionEnemigo,
                 velocidadEnemigo,
                 spriteBatch);
            listaEnemigos.Add(nuevoEnemigo);            
            imagenEnemigo.Dispose();
            GC.Collect();
        }
        private void DetectarColision()
        {
            for(int i = 0; i < listaEnemigos.Count; i++)
            {
                for (int f = 0; f < listaDisparos.Count; f++)
                {
                    if (listaEnemigos[i].rect.Intersects(listaDisparos[f].rect))
                    {
                        listaEnemigos[i].Eliminar();
                        listaDisparos[f].Eliminar();
                        puntaje++;
                        break;
                    }
                }                
            }
        }

        private void Finalizar()
        {
            if (vidas <= 0)
            {
                colision = true;
            }
        }
        private void ReducidirVida()
        {
            for (int i = 0; i < listaEnemigos.Count; i++)
            {
                if (rect.Intersects(listaEnemigos[i].rect))
                {
                    listaEnemigos[i].Eliminar();
                    vidas--;
                    break;
                }
            }
        }
        private void MovimientosTeclado()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                rect.X += 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                rect.X -= 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                rect.Y -= 5;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                rect.Y += 5;
            }
        }

        private void CrearEsfera()
        {
            FileStream imagenEsfera;

            if (puntaje != 0 && (puntaje % 5)==0)
            {
                imagenEsfera = new FileStream("Content/Esferas.png", FileMode.Open);
                Texture2D texEsfera = Texture2D.FromStream(GraphicsDevice, imagenEsfera);
                Vector2 posicionEsfera = new Vector2(1150, random.Next(0, height));
                Vector2 velocidadEsfera = new Vector2(0, random.Next(1, 3));
                Esfera nuevaEsfera = new Esfera(
                     texEsfera,
                     posicionEsfera,
                     velocidadEsfera,
                     spriteBatch);
                listaEsferas.Add(nuevaEsfera);
                imagenEsfera.Dispose();
                GC.Collect();
            }
        }

        private void DibujarEsferas()
        {
            for (int i = 0; i < listaEsferas.Count; i++)
            {
                listaEsferas[i].Draw();
            }
        }

        private void EliminarEsferas()
        {
            for (int i = 0; i < listaEsferas.Count; i++)
            {
                if (listaEsferas[i].rect.X < 50)
                {
                    listaEsferas[i].Eliminar();
                }
            }
        }
        private void ComprobarVidaEsfera()
        {
            for (int i = 0; i < listaEsferas.Count; i++)
            {
                if (!listaEsferas[i].estaVivo)
                {
                    listaEsferas.Remove(listaEsferas[i]);
                }
            }
        }

        private void DetectarColisionEsfera()
        {
            for (int i = 0; i < listaEsferas.Count; i++)
            {
                if (rect.Intersects(listaEsferas[i].rect))
                 {
                        listaEsferas[i].Eliminar();
                        vidas++;
                        break;
                 }
            }
        }

        private void ActualizarEsferas()
        {
            for (int i = 0; i < listaEsferas.Count; i++)
            {
                listaEsferas[i].Update();
            }
        }

    }
}