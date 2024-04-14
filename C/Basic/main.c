#include <stdio.h>
#include "mir_toolkit/mir_client_library.h"

int running = 1;

int BYTES_PER_PIXEL(enum MirPixelFormat format)
{
	switch (format)
	{
		case mir_pixel_format_abgr_8888:
		case mir_pixel_format_xbgr_8888:
		case mir_pixel_format_argb_8888:
		case mir_pixel_format_xrgb_8888:
			return 4;

		case mir_pixel_format_bgr_888:
		case mir_pixel_format_rgb_888:
			return 3;

		case mir_pixel_format_rgb_565:
		case mir_pixel_format_rgba_5551:
		case mir_pixel_format_rgba_4444:
			return 2;
	}
    return 0;
}

static MirOutput const* find_active_output(MirDisplayConfig const* conf)
{
    int num_outputs = mir_display_config_get_num_outputs(conf);
    for (int i = 0; i < num_outputs; i++)
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

    // validate RGBA8 format exists
    MirPixelFormat pixel_format = mir_pixel_format_invalid;
    size_t num_pfs = mir_output_get_num_pixel_formats(output);

    int hasFormat_abgr = 0;
    int hasFormat_xbgr = 0;
    int hasFormat_argb = 0;
    int hasFormat_xrgb = 0;
    for (size_t i = 0; i < num_pfs; i++)
    {
        MirPixelFormat f = mir_output_get_pixel_format(output, i);
        if (BYTES_PER_PIXEL(f) == 4)
        {
            switch (f)
            {
            	case mir_pixel_format_abgr_8888: hasFormat_abgr = 1; break;
            	case mir_pixel_format_xbgr_8888: hasFormat_xbgr = 1; break;
            	case mir_pixel_format_argb_8888: hasFormat_argb = 1; break;
            	case mir_pixel_format_xrgb_8888: hasFormat_xrgb = 1; break;
            }
        }
    }

    int isABGR = 1;
    if (hasFormat_abgr)
    {
        pixel_format = mir_pixel_format_abgr_8888;
        isABGR = 1;
    }
    else if (hasFormat_xbgr)
    {
        pixel_format = mir_pixel_format_xbgr_8888;
        isABGR = 1;
    }
    else if (hasFormat_argb)
    {
        pixel_format = mir_pixel_format_argb_8888;
        isABGR = 0;
    }
    else if (hasFormat_xrgb)
    {
        pixel_format = mir_pixel_format_xrgb_8888;
        isABGR = 0;
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
    mir_window_spec_set_name(spec, "Mir C#");
    mir_window_spec_set_buffer_usage(spec, mir_buffer_usage_software);

    MirWindow* window = mir_create_window_sync(spec);
    mir_window_spec_release(spec);
    if (window == NULL)
    {
    	printf("Failed to create Window");
    	return 1;
    }

    // run
    MirBufferStream* bs = mir_window_get_buffer_stream(window);
    mir_buffer_stream_set_swapinterval(bs, 1);
    mir_window_set_event_handler(window, &on_event, NULL);
    while (running)
    {
    	// get buffer
    	MirGraphicsRegion backbuffer;
        mir_buffer_stream_get_graphics_region(bs, &backbuffer);

        // clear buffer
        char* data = backbuffer.vaddr;
        int size = backbuffer.width * backbuffer.height * 4;
        char channel0 = isABGR ? 255 : 0;
        char channel2 = isABGR ? 0 : 255;
        for (int i = 0; i < size; i += 4)
        {
        	data[i + 0] = channel0;
        	data[i + 1] = 0;
        	data[i + 2] = channel2;// R or B
        	data[i + 3] = 255;// alpha
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

