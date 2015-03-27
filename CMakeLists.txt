# Install a Unity project that represents the Myo SDK for Unity.
# We combine the existing unity project in the project/ directory, which contains things like Unity-specific scripts,
# sample scenes, and assets, with the .NET bindings, plugin shared libraries, and any other additional bits that need
# to be added to it.

set(unity_install ${CMAKE_BINARY_DIR}/sdk_unity)

install(DIRECTORY project/ DESTINATION ${unity_install}/MyoUnity
        PATTERN "Temp" EXCLUDE
        PATTERN "obj" EXCLUDE
        PATTERN "library" EXCLUDE
        PATTERN "*.csproj" EXCLUDE
        PATTERN "*.unityproj" EXCLUDE
        PATTERN "*.sln" EXCLUDE
    )

if(APPLE)
    # Install the plugin bundle
    install(FILES Info.plist DESTINATION ${unity_install}/MyoUnity/Assets/Plugins/myo.bundle/Contents)

    # We use CODE with cmake -E copy here instead of just, say, FILES, in order to resolve symlinks.
    get_target_property(myo_location myo_client LOCATION)
    install(CODE "
                execute_process(COMMAND ${CMAKE_COMMAND} -E copy
                    ${myo_location}
                    ${unity_install}/MyoUnity/Assets/Plugins/myo.bundle/Contents/MacOS/libmyo.dylib)
            ")
elseif(WIN32)
    if(${ARCH} STREQUAL "x64")
        set(plugin_dest "x86_64")
    else()
        set(plugin_dest "x86")
    endif()

    foreach (config ${CMAKE_CONFIGURATION_TYPES})
        install(FILES $<TARGET_FILE:myo_client> DESTINATION ${unity_install}/MyoUnity/Assets/Plugins/${plugin_dest}/
                RENAME myo.dll CONFIGURATIONS ${config})
    endforeach()
endif()

# Install the .NET bindings
install(DIRECTORY "${CMAKE_SOURCE_DIR}/myo-dotnet/myo-dotnet/"
        DESTINATION "${unity_install}/MyoUnity/Assets/Myo/Scripts/Myo.NET"
        FILES_MATCHING PATTERN "*.cs"
)
