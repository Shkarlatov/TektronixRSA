namespace Tektronix.TekRSA
{
    public enum ReturnStatus
    {
        // User errors

        successful = 0,    // most API functions return this value when successful

        // Connection
        errorNotConnected = 101,
        errorIncompatibleFirmware,
        errorBootLoaderNotRunning,
        errorTooManyBootLoadersConnected,
        errorRebootFailure,
        errorGNSSNotInstalled,
        errorGNSSNotEnabled,

        // POST
        errorPOSTFailureFPGALoad = 201,
        errorPOSTFailureHiPower,
        errorPOSTFailureI2C,
        errorPOSTFailureGPIF,
        errorPOSTFailureUsbSpeed,
        errorPOSTDiagFailure,
        errorPOSTFailure3P3VSense,

        // General Msmt
        errorBufferAllocFailed = 301,
        errorParameter,
        errorDataNotReady,

        // Spectrum
        errorParameterTraceLength = 1101,
        errorMeasurementNotEnabled,
        errorSpanIsLessThanRBW,
        errorFrequencyOutOfRange,

        // IF streaming
        errorStreamADCToDiskFileOpen = 1201,
        errorStreamADCToDiskAlreadyStreaming,
        errorStreamADCToDiskBadPath,
        errorStreamADCToDiskThreadFailure,
        errorStreamedFileInvalidHeader,
        errorStreamedFileOpenFailure,
        errorStreamingOperationNotSupported,
        errorStreamingFastForwardTimeInvalid,
        errorStreamingInvalidParameters,
        errorStreamingEOF,
        errorStreamingIfReadTimeout,
        errorStreamingIfNotEnabled,

        // IQ streaming
        errorIQStreamInvalidFileDataType = 1301,
        errorIQStreamFileOpenFailed,
        errorIQStreamBandwidthOutOfRange,
        errorIQStreamingNotEnabled,



        // Internal errors
        errorTimeout = 3001,
        errorTransfer,
        errorFileOpen,
        errorFailed,
        errorCRC,
        errorChangeToFlashMode,
        errorChangeToRunMode,
        errorDSPLError,
        errorLOLockFailure,
        errorExternalReferenceNotEnabled,
        errorLogFailure,
        errorRegisterIO,
        errorFileRead,
        errorConsumerNotActive,

        errorDisconnectedDeviceRemoved = 3101,
        errorDisconnectedDeviceNodeChangedAndRemoved,
        errorDisconnectedTimeoutWaitingForADcData,
        errorDisconnectedIOBeginTransfer,
        errorOperationNotSupportedInSimMode,
        errorDisconnectedIOFinishTransfer,

        errorFPGAConfigureFailure = 3201,
        errorCalCWNormFailure,
        errorSystemAppDataDirectory,
        errorFileCreateMRU,
        errorDeleteUnsuitableCachePath,
        errorUnableToSetFilePermissions,
        errorCreateCachePath,
        errorCreateCachePathBoost,
        errorCreateCachePathStd,
        errorCreateCachePathGen,
        errorBufferLengthTooSmall,
        errorRemoveCachePath,
        errorGetCachingDirectoryBoost,
        errorGetCachingDirectoryStd,
        errorGetCachingDirectoryGen,
        errorInconsistentFileSystem,

        errorWriteCalConfigHeader = 3301,
        errorWriteCalConfigData,
        errorReadCalConfigHeader,
        errorReadCalConfigData,
        errorEraseCalConfig,
        errorCalConfigFileSize,
        errorInvalidCalibConstantFileFormat,
        errorMismatchCalibConstantsSize,
        errorCalConfigInvalid,

        // flash
        errorFlashFileSystemUnexpectedSize = 3401,
        errorFlashFileSystemNotMounted,
        errorFlashFileSystemOutOfRange,
        errorFlashFileSystemIndexNotFound,
        errorFlashFileSystemReadErrorCRC,
        errorFlashFileSystemReadFileMissing,
        errorFlashFileSystemCreateCacheIndex,
        errorFlashFileSystemCreateCachedDataFile,
        errorFlashFileSystemUnsupportedFileSize,
        errorFlashFileSystemInsufficentSpace,
        errorFlashFileSystemInconsistentState,
        errorFlashFileSystemTooManyFiles,
        errorFlashFileSystemImportFileNotFound,
        errorFlashFileSystemImportFileReadError,
        errorFlashFileSystemImportFileError,
        errorFlashFileSystemFileNotFoundError,
        errorFlashFileSystemReadBufferTooSmall,
        errorFlashWriteFailure,
        errorFlashReadFailure,
        errorFlashFileSystemBadArgument,
        errorFlashFileSystemCreateFile,

        // Aux monitoring
        errorMonitoringNotSupported = 3501,
        errorAuxDataNotAvailable,

        // battery
        errorBatteryCommFailure = 3601,
        errorBatteryChargerCommFailure = 3602,
        errorBatteryNotPresent = 3603,

        //EST
        errorESTOutputPathFile = 3701,
        errorESTPathNotDirectory,
        errorESTPathDoesntExist,
        errorESTUnableToOpenLog,
        errorESTUnableToOpenLimits,

        // Revision information
        errorRevisionDataNotFound = 3801,

        // alignment
        error112MHzAlignmentSignalLevelTooLow = 3901,
        error10MHzAlignmentSignalLevelTooLow,
        errorInvalidCalConstant,
        errorNormalizationCacheInvalid,
        errorInvalidAlignmentCache,
        errorLockExtRefAfterAlignment,

        // Triggering
        errorTriggerSystem = 4000,

        // VNA
        errorVNAUnsupportedConfiguration = 4100,

        // MFC
        errorMFCHWNotPresent = 4200,
        errorMFCWriteCalFile,
        errorMFCReadCalFile,
        errorMFCFileFormatError,
        errorMFCFlashCorruptDataError,

        // acq status
        errorADCOverrange = 9000,	// must not change the location of these error codes without coordinating with MFG TEST
        errorOscUnlock = 9001,

        errorNotSupported = 9901,

        errorPlaceholder = 9999,
        notImplemented = -1
    }
}
