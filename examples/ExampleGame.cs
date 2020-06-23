using System;
using System.Collections.Generic;

using Strike;

namespace StrikeTest
{
    using static StrikeGame;

    class Bullet {
        public Vector pos;
        public Vector dir;

        public void Update() {
            pos += dir * 600.0f * Game.Delta;
        }
        public void Render() {
            Game.Canvas.DrawRect(
                new Rect { x = pos.x - 5, y = pos.y - 5, w = 10, h = 10 },
                new Color { r = 0xff, g = 0xff, b = 0xff, a = 0xff },
                filled: true
            );
        }
    }
    
    class Guy {
        Sprite sprite;
        Vector pos = Vector.Zero;
        List<Bullet> bullets;

        public Guy() {
            sprite = Game.Canvas.LoadSpriteFromFile("guy.png");
            bullets = new List<Bullet>();
        }

        public void Update() {
            // Move
            var move = Vector.Zero;
            move.x += Game.Input.KeyDown(Key.A) ? -1.0f : 0.0f;
            move.x += Game.Input.KeyDown(Key.D) ? +1.0f : 0.0f;
            move.y += Game.Input.KeyDown(Key.W) ? -1.0f : 0.0f;
            move.y += Game.Input.KeyDown(Key.S) ? +1.0f : 0.0f;

            pos += move * 500.0f * Game.Delta;

            // Shoot
            if (Game.Input.MouseButtonPressed(MouseButton.LEFT)) {
                var relative_mouse = Game.Canvas.ScreenToWorld(Game.Input.MousePos()) - pos;
                var bullet_pos = pos + relative_mouse.Normalize() * 45.0f;
                bullets.Add(new Bullet {
                        pos = bullet_pos,
                        dir = relative_mouse.Normalize(),
                    });
            }

            // Bullets
            foreach (var bullet in bullets) {
                bullet.Update();
            }
        }
        
        public void Render() {
            var relative_mouse = Game.Canvas.ScreenToWorld(Game.Input.MousePos()) - pos;
            float angle = (float) Math.Atan2(relative_mouse.y, relative_mouse.x);
            angle += (float) (Math.PI / 2.0);
            
            Game.Canvas.DrawSprite(sprite, pos, angle: angle);

            foreach (var bullet in bullets) {
                bullet.Render();
            }
        }
    }
    
    class TestGame : StrikeGame {
        Guy guy;
        
        public TestGame()
            : base("Sample", 800, 600) {}
        protected override void Init() {
            guy = new Guy();
        }
        protected override void Update() {
            if (Input.KeyPressed(Key.ESCAPE)) {
                Quit();
            }

            guy.Update();
        }
        protected override void Render() {
            Canvas.Clear(new Color { r = 0, g = 0, b = 0, a = 0xff });
            
            guy.Render();
            
            Canvas.Present();
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var game = new TestGame();
            game.Run();
        }
    }
}
