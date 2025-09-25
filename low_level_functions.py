import numpy as np
def Conv2D(img_in, filters): # Shape : (N, width, height)
  # Calculate activation shape and filter shape
    number_of_images = img_in.shape[0]
    W_out = int(np.floor((img_in.shape[1] - filters.shape[1] + 2 * 0) / 1 + 1))
    H_out = int(np.floor((img_in.shape[2] - filters.shape[2] + 2 * 0) / 1 + 1))

    number_of_filters = filters.shape[0]
    filter_size_x = filters.shape[1]
    filter_size_y = filters.shape[2]
    filter_size = filter_size_x * filter_size_y
    
    # Determine window positions
    sliding_x_positions = np.arange(0, W_out, 1)
    sliding_y_positions = np.arange(0, H_out, 1)

    kernel_x = np.arange(0,filter_size_x,1)
    kernel_y = np.arange(0,filter_size_y,1)

    # Reshape to 4D for broadcasting
    sliding_x_positions =  sliding_x_positions.reshape(-1, 1, 1, 1)
    sliding_y_positions =  sliding_y_positions.reshape(1, -1, 1, 1)
    kernel_x = kernel_x.reshape(1, 1, -1, 1)
    kernel_y = kernel_y.reshape(1, 1, 1, -1)

    # Create indexes of windows    
    x_idx = sliding_x_positions + kernel_x
    x_idx = x_idx.astype(np.int32)

    y_idx = sliding_y_positions + kernel_y
    y_idx = y_idx.astype(np.int32)

    batch_idx = np.arange(number_of_images).reshape(number_of_images, 1, 1, 1, 1)
    
    # Create windowed input (index the input image)
    windowed_input = img_in[batch_idx, x_idx, y_idx].reshape(number_of_images, W_out * H_out, filter_size)
    
    # Multiply by filters
    filters_windowed = np.tile(filters.reshape(number_of_images, filter_size), windowed_input.shape[1])
    filters_windowed = filters_windowed.reshape(number_of_images,windowed_input.shape[1], filter_size)
    input_filtered = windowed_input * filters_windowed

    # Create activation map
    activation_maps = np.sum(input_filtered, axis = 2).reshape(number_of_images, W_out, H_out)
    return activation_maps

def ReLU(activation_map): #  Shape = (N, width, height)
    return (activation_map > 0) * activation_map

def MaxPool2D(img_in, filter_size): # Shape (N, width, height)'
    
  # Calculate image shape, activation shape and filter shape
    number_of_images = img_in.shape[0]
    img_width = img_in.shape[1]
    img_height = img_in.shape[2]

    filter_size_ = filter_size * filter_size
    W_out = int(np.floor(img_width / filter_size))
    H_out = int(np.floor(img_height / filter_size))

    
    # Determine window positions
    sliding_x_positions = np.arange(0, img_width - (filter_size - 1) , filter_size)
    sliding_y_positions = np.arange(0, img_height - (filter_size - 1), filter_size)

    kernel_x = np.arange(0,filter_size,1)
    kernel_y = np.arange(0,filter_size,1)

    # Reshape to 4D for broadcasting
    sliding_x_positions =  sliding_x_positions.reshape(-1, 1, 1, 1)
    sliding_y_positions =  sliding_y_positions.reshape(1, -1, 1, 1)
    kernel_x = kernel_x.reshape(1, 1, -1, 1)
    kernel_y = kernel_y.reshape(1, 1, 1, -1)
    batch_idx = np.arange(number_of_images).reshape(number_of_images, 1, 1, 1, 1)

    # Create windows
    x_idx = sliding_x_positions + kernel_x
    x_idx = x_idx.astype(np.int32)

    y_idx = sliding_y_positions + kernel_y
    y_idx = y_idx.astype(np.int32)
    
    # Pool windows
    windowed_input = img_in[batch_idx, x_idx, y_idx]
    pooled_output = np.max(windowed_input.reshape(number_of_images, W_out * H_out, filter_size_), axis = 2).reshape(number_of_images, W_out, H_out)
    return pooled_output

def Normalize(activation_map): # Shape: (N, width, height)
    mean = np.mean(activation_map, axis=(1, 2), keepdims=True)
    std = np.std(activation_map, axis=(1, 2), keepdims=True)
    return (activation_map - mean) / (std + 1e-8)  # No divison by 0


def FullyConnectedLayer(activation_maps, weights):
    print(activation_maps.shape)
    flatten = activation_maps.reshape(-1)
    output = flatten @ weights # No bias included
    return output

def SoftMax(activation_map):
    exp = np.exp(activation_map)
    return exp / np.sum(exp)