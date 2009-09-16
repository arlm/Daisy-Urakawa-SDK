package org.daisy.urakawa.media;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.event.media.ClipChangedEvent;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.timing.Time;
import org.daisy.urakawa.media.timing.TimeDelta;
import org.daisy.urakawa.media.timing.TimeImpl;
import org.daisy.urakawa.media.timing.TimeOffsetIsOutOfBoundsException;
import org.daisy.urakawa.media.timing.TimeStringRepresentationIsInvalidException;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.progress.ProgressHandler;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ExternalAudioMediaImpl extends ExternalMediaAbstractImpl implements
		AudioMedia, Clipped {
	private Time mClipBegin;
	private Time mClipEnd;

	@Override
	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (ClipChangedEvent.class.isAssignableFrom(event.getClass())) {
			mClipChangedEventNotifier.notifyListeners(event);
		}
		super.notifyListeners(event);
	}

	@Override
	public <K extends DataModelChangedEvent> void registerListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (ClipChangedEvent.class.isAssignableFrom(klass)) {
			mClipChangedEventNotifier.registerListener(listener, klass);
		} else {
			super.registerListener(listener, klass);
		}
	}

	@Override
	public <K extends DataModelChangedEvent> void unregisterListener(
			EventListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (ClipChangedEvent.class.isAssignableFrom(klass)) {
			mClipChangedEventNotifier.unregisterListener(listener, klass);
		} else {
			super.unregisterListener(listener, klass);
		}
	}

	protected EventHandler<Event> mClipChangedEventNotifier = new EventHandlerImpl();

	private void resetClipTimes() {
		mClipBegin = new TimeImpl().getZero();
		mClipEnd = new TimeImpl().getMaxValue();
	}

	protected ExternalAudioMediaImpl() {
		resetClipTimes();
	}

	@Override
	public boolean isContinuous() {
		return true;
	}

	@Override
	public boolean isDiscrete() {
		return false;
	}

	@Override
	public boolean isSequence() {
		return false;
	}

	@Override
	public ExternalAudioMediaImpl copy() {
		return (ExternalAudioMediaImpl) copyProtected();
	}

	@Override
	public ExternalAudioMediaImpl export(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return (ExternalAudioMediaImpl) exportProtected(destPres);
	}

	@Override
	protected Media exportProtected(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		ExternalAudioMediaImpl exported = (ExternalAudioMediaImpl) super
				.exportProtected(destPres);
		if (exported == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			exported.setClipBegin(getClipBegin().copy());
			exported.setClipEnd(getClipEnd().copy());
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return exported;
	}

	@Override
	protected void xukInAttributes(XmlDataReader source, ProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}

		// To avoid event notification overhead, we bypass this:
		if (false && ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		super.xukInAttributes(source, ph);
		resetClipTimes();
		Time cbTime, ceTime;
		try {
			cbTime = new TimeImpl(source.getAttribute("clipBegin"));
			ceTime = new TimeImpl(source.getAttribute("clipEnd"));
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (TimeStringRepresentationIsInvalidException e) {
			throw new XukDeserializationFailedException();
		}
		try {
			if (cbTime.isNegativeTimeOffset()) {
				setClipBegin(cbTime);
				setClipEnd(ceTime);
			} else {
				setClipEnd(ceTime);
				setClipBegin(cbTime);
			}
		} catch (TimeOffsetIsOutOfBoundsException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri,
			ProgressHandler ph) throws MethodParameterIsNullException,
			XukSerializationFailedException, ProgressCancelledException {
		if (destination == null || baseUri == null) {
			throw new MethodParameterIsNullException();
		}
		if (ph != null && ph.notifyProgress()) {
			throw new ProgressCancelledException();
		}
		destination.writeAttributeString("clipBegin", this.getClipBegin()
				.toString());
		destination.writeAttributeString("clipEnd", this.getClipEnd()
				.toString());
		super.xukOutAttributes(destination, baseUri, ph);
	}

	public TimeDelta getDuration() {
		try {
			return getClipEnd().getTimeDelta(getClipBegin());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public Time getClipBegin() {
		return mClipBegin;
	}

	public Time getClipEnd() {
		return mClipEnd;
	}

	public void setClipBegin(Time beginPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (beginPoint == null) {
			throw new MethodParameterIsNullException();
		}
		if (beginPoint.isLessThan(new TimeImpl().getZero())) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (beginPoint.isGreaterThan(getClipEnd())) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (!mClipBegin.isEqualTo(beginPoint)) {
			Time prevCB = getClipBegin();
			mClipBegin = beginPoint.copy();
			notifyListeners(new ClipChangedEvent(this, getClipBegin(),
					getClipEnd(), prevCB, getClipEnd()));
		}
	}

	public void setClipEnd(Time endPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (endPoint == null) {
			throw new MethodParameterIsNullException();
		}
		if (endPoint.isLessThan(getClipBegin())) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (!mClipEnd.isEqualTo(endPoint)) {
			Time prevCE = getClipEnd();
			mClipEnd = endPoint.copy();
			notifyListeners(new ClipChangedEvent(this, getClipBegin(),
					getClipEnd(), getClipBegin(), prevCE));
		}
	}

	public ExternalAudioMediaImpl split(Time splitPoint)
			throws MethodParameterIsNullException,
			TimeOffsetIsOutOfBoundsException {
		if (splitPoint == null) {
			throw new MethodParameterIsNullException();
		}
		if (splitPoint.isLessThan(getClipBegin())) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		if (splitPoint.isGreaterThan(getClipEnd())) {
			throw new TimeOffsetIsOutOfBoundsException();
		}
		ExternalAudioMediaImpl splitAM = (ExternalAudioMediaImpl) copy();
		setClipEnd(splitPoint);
		splitAM.setClipBegin(splitPoint);
		return splitAM;
	}

	@Override
	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {
		if (!super.ValueEquals(other))
			return false;
		ExternalAudioMediaImpl otherAudio = (ExternalAudioMediaImpl) other;
		if (!getClipBegin().isEqualTo(otherAudio.getClipBegin()))
			return false;
		if (!getClipEnd().isEqualTo(otherAudio.getClipEnd()))
			return false;
		return true;
	}
}