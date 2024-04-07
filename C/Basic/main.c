﻿#include <stdio.h>
#include "mir_toolkit/mir_client_library.h"

#define BYTES_PER_PIXEL(f) ((f) == mir_pixel_format_bgr_888 ? 3 : 4)
int running = 1;

static MirOutput const* find_active_output(MirDisplayConfig const* conf)
{
    size_t num_outputs = mir_display_config_get_num_outputs(conf);
    for (size_t i = 0; i < num_outputs; i++)
    {
        MirOutput const* output = mir_display_config_get_output(conf, i);
        MirOutputConnectionState state = mir_output_get_connection_state(output);
        if (state == mir_output_connection_state_connected && mir_output_is_enabled(output))
        {
            return output;
        }
    }

    return NULL;
}

static void on_event(MirWindow *surface, const MirEvent *event, void *context)
{
	MirEventType event_type = mir_event_get_type(event);
	if (event_type == mir_event_type_close_window) running = 0;
}

int main (int argc, char *argv[])
{
	printf ("Hello Mir!\n");

	char *mir_socket = NULL;
	int swap_interval = 0;

	// get connection
	MirConnection *conn;
	conn = mir_connect_sync(mir_socket, argv[0]);
	if (!mir_connection_is_valid(conn))
    {
        fprintf(stderr, "Could not connect to a display server: %s\n", mir_connection_get_error_message(conn));
        return 1;
    }

    // get display config
    MirDisplayConfig* display_config = mir_connection_create_display_configuration(conn);

    // get display output
    MirOutput const* output = find_active_output(display_config);
    if (output == NULL)
    {
        fprintf(stderr, "No active outputs found.\n");
        mir_connection_release(conn);
        return 1;
    }

    // validate RGBA8 format
    MirPixelFormat pixel_format = mir_pixel_format_invalid;
    size_t num_pfs = mir_output_get_num_pixel_formats(output);

    for (size_t i = 0; i < num_pfs; i++)
    {
        MirPixelFormat f = mir_output_get_pixel_format(output, i);
        if (BYTES_PER_PIXEL(f) == 4)
        {
            pixel_format = f;
            break;
        }
    }

    if (pixel_format == mir_pixel_format_invalid)
    {
        fprintf(stderr, "Could not find a fast 32-bit pixel format\n");
        mir_connection_release(conn);
        return 1;
    }

    // get display size
    MirOutputMode const* mode = mir_output_get_current_mode(output);
    int width  = mir_output_mode_get_width(mode);
    int height = mir_output_mode_get_height(mode);
    mir_display_config_release(display_config);

    // create window
	MirWindowSpec *spec = mir_create_normal_window_spec(conn, width, height);
    mir_window_spec_set_pixel_format(spec, pixel_format);
    mir_window_spec_set_name(spec, "Basic Window");
    mir_window_spec_set_buffer_usage(spec, mir_buffer_usage_software);

    MirWindow* window;
    window = mir_create_window_sync(spec);
    mir_window_spec_release(spec);
    if (window == NULL)
    {
    	printf("Failed to create Window");
    	return 1;
    }

    // run
    MirBufferStream* bs = mir_window_get_buffer_stream(window);
    mir_buffer_stream_set_swapinterval(bs, swap_interval);
    mir_window_set_event_handler(window, &on_event, NULL);
    while (running)
    {
    	// get buffer
    	MirGraphicsRegion backbuffer;
        mir_buffer_stream_get_graphics_region(bs, &backbuffer);

        // clear buffer
        char* data = backbuffer.vaddr;
        int size = backbuffer.width * backbuffer.height * 4;
        for (int i = 0; i < size; i += 4)
        {
        	data[i + 0] = 0;
        	data[i + 1] = 0;
        	data[i + 2] = 255;
        	data[i + 3] = 255;
        }

        // swap buffer
        mir_buffer_stream_swap_buffers_sync(bs);
    }

    // shutdown
    mir_window_set_event_handler(window, NULL, NULL);
    mir_window_release_sync(window);
    mir_connection_release(conn);
	
	return 0;
}
