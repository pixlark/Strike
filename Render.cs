using System;
using static SDL2.SDL;

namespace Strike {
    public class Sprite {
        public IntPtr handle;
        public VectorInt size;
        public Sprite(IntPtr handle) {
            this.handle = handle;
            
            uint fmt; int access; // Junk for out parameters
            var size = VectorInt.Zero;
            
            var err = SDL_QueryTexture(
                handle, out fmt, out access,
                out size.x, out size.y
            );
            if (err != 0) {
                throw new SDLException(SDL_GetError());
            }
            
            this.size = size;
        }
    }

    public class Canvas {
        IntPtr renderer;
        Rect camera_view;

        public int screen_width;
        public int screen_height;
        
        public Canvas(IntPtr renderer, int width, int height) {
            this.renderer = renderer;
            this.screen_width = width;
            this.screen_height = height;
            this.camera_view = new Rect {
                x = -((float) width)  / 2,
                y = -((float) height) / 2,
                w = (float) width,
                h = (float) height,
            };
        }
        public void Clear(Color color) {
            int err;
            err = SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
            if (err != 0) {
                throw new SDLException(SDL_GetError());
            }
            err = SDL_RenderClear(renderer);
            if (err != 0) {
                throw new SDLException(SDL_GetError());
            }
        }
        public void Present() {
            SDL_RenderPresent(renderer);
        }
        public Sprite LoadSpriteFromFile(string path) {
            var surface = SDL2.SDL_image.IMG_Load(path);
            if (surface == null) {
                throw new System.IO.FileNotFoundException();
            }
            var sprite = SDL_CreateTextureFromSurface(renderer, surface);
            if (sprite == null) {
                throw new SDLException(SDL_GetError());
            }
            SDL_FreeSurface(surface);
            return new Sprite(sprite);
        }
        public VectorInt WorldToScreen(Vector vec) {
            return new VectorInt {
                x = (int) (vec.x - camera_view.x),
                y = (int) (vec.y - camera_view.y),
            };
        }
        public RectInt WorldToScreen(Rect rect) {
            return new RectInt {
                x = (int) (rect.x - camera_view.x),
                y = (int) (rect.y - camera_view.y),
                w = (int) rect.w,
                h = (int) rect.h,
            };
        }
        public Vector ScreenToWorld(VectorInt vec) {
            return new Vector {
                x = vec.x + camera_view.x,
                y = vec.y + camera_view.y,
            };
        }
        public Rect ScreenToWorld(RectInt rect) {
            return new Rect {
                x = rect.x + camera_view.x,
                y = rect.y + camera_view.y,
                w = rect.w,
                h = rect.h,
            };
        }
        public void MoveCamera(Vector movement) {
            camera_view.x += movement.x;
            camera_view.y += movement.y;
        }
        public void DrawRect(Rect rect, Color color, bool filled = true) {
            var screen_rect = WorldToScreen(rect);
            var sdl_rect = new SDL_Rect {
                x = screen_rect.x,
                y = screen_rect.y,
                w = screen_rect.w,
                h = screen_rect.h,
            };
            SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
            if (filled) {
                SDL_RenderFillRect(
                    renderer, ref sdl_rect
                );
            } else {
                SDL_RenderDrawRect(
                    renderer, ref sdl_rect
                );
            }
        }
        public void DrawSprite(Sprite sprite, Vector pos, bool centered = true, float angle = 0.0f) {
            DrawSprite(
                sprite,
                new Rect {
                    x = pos.x - sprite.size.x / 2,
                    y = pos.y - sprite.size.y / 2,
                    w = sprite.size.x,
                    h = sprite.size.y
                },
                angle
            );
        }
        public void DrawSprite(Sprite sprite, Rect rect, float angle = 0.0f) {
            var src_rect = new SDL_Rect {
                x = 0,
                y = 0,
                w = sprite.size.x,
                h = sprite.size.y,
            };

            var screen_rect = WorldToScreen(rect);
            var dest_rect = new SDL_Rect {
                x = screen_rect.x,
                y = screen_rect.y,
                w = screen_rect.w,
                h = screen_rect.h,
            };

            int err;
            
            if (angle == 0.0f) {
                err = SDL_RenderCopy(
                    renderer, sprite.handle,
                    ref src_rect, ref dest_rect
                );
                if (err != 0) {
                    throw new SDLException(SDL_GetError());
                }
                return;
            }

            var rotation_origin = new SDL_Point {
                x = screen_rect.w / 2,
                y = screen_rect.h / 2,
            };
            err = SDL_RenderCopyEx(
                renderer, sprite.handle,
                ref src_rect, ref dest_rect,
                (double) angle * 180.0 / Math.PI,
                ref rotation_origin,
                SDL_RendererFlip.SDL_FLIP_NONE
            );
            if (err != 0) {
                throw new SDLException(SDL_GetError());
            }
        }
    }
}
