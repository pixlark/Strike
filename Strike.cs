using System;
using static SDL2.SDL;

namespace Strike {
    public class SDLException : Exception {
        public SDLException() {}
        public SDLException(string message) : base(message) {}
        public SDLException(string message, Exception inner) : base(message, inner) {}
    }

    public class StrikeGame {
        IntPtr window;
        Canvas canvas;
        
        ulong last_tick;
        float last_delta;

        InputState input_state;

        bool quitting = false;

        public Canvas Canvas { get => canvas; }
        public InputState Input { get => input_state; }
        public float Delta { get => last_delta; }

        static StrikeGame instance = null;
        static void SetInstance(StrikeGame game) {
            if (instance != null) {
                throw new InvalidOperationException("Current game instance is already set");
            }
            instance = game;
        }
        public static StrikeGame Game {
            get {
                if (instance == null) {
                    throw new InvalidOperationException("Current game instance is not set");
                }
                return instance;
            }
        }
        
        protected StrikeGame(string title, int width, int height) {
            SetInstance(this);
            
            int err;
            
            err = SDL_Init(SDL_INIT_EVERYTHING);
            if (err != 0) {
                throw new SDLException(SDL_GetError());
            }

            window = SDL_CreateWindow(
                title, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
                width, height, SDL_WindowFlags.SDL_WINDOW_SHOWN
            );
            if (window == null) {
                throw new SDLException(SDL_GetError());
            }
            
            var renderer = SDL_CreateRenderer(window, -1, 0);
            if (renderer == null) {
                throw new SDLException(SDL_GetError());
            }

            canvas = new Canvas(renderer, width, height);

            last_tick = SDL_GetPerformanceCounter();
            last_delta = 1.0f / 60.0f;

            input_state = new InputState();
        }
        
        public void Run() {
            Init();
            
            SDL_Event sdl_event;
            while (true) {
                if (quitting) {
                    break;
                }
                
                while (SDL_PollEvent(out sdl_event) != 0) {
                    if (sdl_event.type == SDL_EventType.SDL_QUIT) {
                        goto end;
                    }
                }
                
                input_state.Update();
                
                Update();
                
                Render();

                {
                    var tick = SDL_GetPerformanceCounter();
                    last_delta = (float) (tick - last_tick) / SDL_GetPerformanceFrequency();
                    last_tick = tick;
                }
            }
        end:
            return;
        }

        public void Quit() {
            quitting = true;
        }
        
        protected virtual void Init() {}
        protected virtual void Update() {}
        protected virtual void Render() {}
    }
}
